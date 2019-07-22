using DomainObjects;
using Repository.QueuedTasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CachedRepository<TEntity> : Repository<TEntity>
        where TEntity : Identity
    {
        private static List<Identity> _cache;

        private static List<Type> _fullyCachedEntities;

        private static ConcurrentQueue<IQueuedTask> _taskQueue;

        private static Task _taskExecutor;

        private static Mutex mut = new Mutex();

        public CachedRepository(DatabaseContext context)
            : base(context)
        {
            if (_taskExecutor == null)
            {
                _taskExecutor = new Task(() =>
                {
                    while (true)
                    {
                        var taskQueue = GetTaskQueue();
                        if (!taskQueue.IsEmpty)
                        {
                            taskQueue.TryDequeue(out IQueuedTask task);
                            mut.WaitOne();
                            try
                            {
                                task.Execute();
                            }
                            catch (Exception ex)
                            {
                                _cache = null;
                                _fullyCachedEntities = null;
                                throw ex;
                            }
                            mut.ReleaseMutex();
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                });
                _taskExecutor.Start();
            }
        }

        public override TEntity Create(TEntity item)
        {
            GetCache().Add(item);
            var task = new ItemTask((i) =>
            {
                var result = base.Create(i as TEntity);
                Context.SaveChanges();
                return result;
            }, item);
            GetTaskQueue().Enqueue(task);
            return item;
        }

        public override void CreateMany(IEnumerable<TEntity> list)
        {
            foreach (var item in list)
            {
                GetCache().Add(item);
            }
            var task = new ItemsTask((i) =>
            {
                base.CreateMany(i.Select(t => t as TEntity));
                Context.SaveChanges();
            }, list);
            GetTaskQueue().Enqueue(task);
        }

        public override void Delete(Guid id)
        {
            var item = GetCache().OfType<TEntity>().FirstOrDefault(i => i.Id.Equals(id));
            if (item != null)
                GetCache().Remove(item);
            var task = new GuidTask((identity) =>
            {
                base.Delete(identity);
                Context.SaveChanges();
            }, id);
            GetTaskQueue().Enqueue(task);
        }

        public override void DeleteRange(IEnumerable<TEntity> list)
        {
            var task = new ItemsTask((i) =>
            {
                base.DeleteRange(i.Select(t => t as TEntity));
                Context.SaveChanges();
                GetCache().Clear();
            }, list);
            GetTaskQueue().Enqueue(task);
        }

        public override IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            var predicate = expression.Compile();
            return GetAll().Where(item => predicate(item));
        }

        public override TEntity Get(object id)
        {
            var item = GetCache().OfType<TEntity>().FirstOrDefault(i => i.Id.Equals(id));
            if (item == null)
            {
                item = base.Get(id);
                if (item != null)
                {
                    GetCache().Add(item);
                }
            }
            return item;
        }

        public override IEnumerable<TEntity> GetAll()
        {
            if (GetFullyCachedEneities().Contains(typeof(TEntity)))
            {
                return GetCache().OfType<TEntity>();
            }
            var items = base.GetAll();
            GetCache().AddRange(items);
            GetFullyCachedEneities().Add(typeof(TEntity));
            return items;
        }


        public override void Update(TEntity item)
        {
            var cachedItem = GetCache().OfType<TEntity>().FirstOrDefault(i => i.Id.Equals(item.Id));
            if (cachedItem == null)
            {
                GetCache().Add(item);
            }
            else
            {
                cachedItem = item;
            }
            var task = new ItemTask((i) =>
            {
                base.Update(i as TEntity);
                Context.SaveChanges();
                return null;
            }, item);
            GetTaskQueue().Enqueue(task);
        }

        internal List<Identity> GetCache()
        {
            if (_cache == null)
                _cache = new List<Identity>();
            return _cache;
        }

        internal ConcurrentQueue<IQueuedTask> GetTaskQueue()
        {
            if (_taskQueue == null)
                _taskQueue = new ConcurrentQueue<IQueuedTask>();
            return _taskQueue;
        }

        internal List<Type> GetFullyCachedEneities()
        {
            if (_fullyCachedEntities == null)
                _fullyCachedEntities = new List<Type>();
            return _fullyCachedEntities;
        }
    }
}
