﻿using AutoMapper;
using System.Reflection;

namespace Web.Profiles
{
    internal static class AutoMapperConfig
    {
        private static IMapper _mapper;

        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Маппер
        /// </summary>
        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    lock (SyncRoot)
                    {
                        if (_mapper == null)
                        {
                            var currentAssembly = Assembly.GetAssembly(typeof(AutoMapperConfig));

                            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfiles(currentAssembly));

                            _mapper = AutoMapper.Mapper.Instance;
                        }
                    }
                }

                return _mapper;
            }
        }
    }
}
