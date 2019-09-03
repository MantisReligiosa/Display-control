function MetaBlockEditViewModel(master) {
    var self = this;

    self.id = ko.observable();
    self.caption = ko.observable();
    self.metaFrames = ko.observableArray();
    self.metaFrames.subscribe(function (frames) {
        if (self.currentFrame.Index() == null) {
            self.selectFrame(frames[0]);
        }
    });
    self.currentFrame = {
        Index: ko.observable(),
        Duration: ko.observable(),

        UseInTimeInerval: ko.observable(false),
        UseFromTime: ko.observable(),
        UseToTime: ko.observable(),

        UseInDayOfWeek: ko.observable(false),
        UseInMon: ko.observable(),
        UseInTue: ko.observable(),
        UseInWed: ko.observable(),
        UseInThu: ko.observable(),
        UseInFri: ko.observable(),
        UseInSat: ko.observable(),
        UseInSun: ko.observable(),

        UseInDate: ko.observable(false),
        DateToUse: ko.observable()
    };
    self.currentFrame.UseInDayOfWeek.subscribe(function (value) {
        if (value) {
            self.currentFrame.UseInDate(false);
        }
    });
    self.currentFrame.UseInDate.subscribe(function (value) {
        if (value) {
            self.currentFrame.UseInDayOfWeek(false);
        }
    });

    self.currentFrame.UseInDayOfWeek.subscribe(function (enabled) {
        $('#daysOfWeek').multiselect(enabled ? 'enable' : 'disable');
    });

    self.daysOfWeek = [
        {
            name: 'mon',
            caption: 'Понедельник',
            setState: function (checked) {
                self.currentFrame.UseInMon(checked);
            }
        },
        {
            name: 'tue',
            caption: 'Вторник',
            setState: function (checked) {
                self.currentFrame.UseInTue(checked);
            }
        },
        {
            name: 'wed',
            caption: 'Среда',
            setState: function (checked) {
                self.currentFrame.UseInWed(checked);
            }
        },
        {
            name: 'thu',
            caption: 'Четверг',
            setState: function (checked) {
                self.currentFrame.UseInThu(checked);
            }
        },
        {
            name: 'fri',
            caption: 'Пятница',
            setState: function (checked) {
                self.currentFrame.UseInFri(checked);
            }
        },
        {
            name: 'sat',
            caption: 'Суббота',
            setState: function (checked) {
                self.currentFrame.UseInSat(checked);
            }
        },
        {
            name: 'sun',
            caption: 'Воскресенье',
            setState: function (checked) {
                self.currentFrame.UseInSun(checked);
            }
        }
    ];

    var isDialogInitialized = false;

    self.initializeControls = function () {
        var daysOfWeekPicker = $('#daysOfWeek');
        var timeIntervalFrom = $('#timeIntervalFrom');
        var timeIntervalTo = $('#timeIntervalTo');
        var dateToUse = $('#dateToUse');
        if (
            !daysOfWeekPicker.length ||
            !timeIntervalFrom.length ||
            !timeIntervalTo.length ||
            !dateToUse.length ||
            isDialogInitialized) {
            return;
        }
        isDialogInitialized = true;

        daysOfWeekPicker.multiselect({
            onChange: function (element, checked) {
                var dayOfWeek = self.daysOfWeek.find(function (dayOfWeek) {
                    return dayOfWeek.name === element[0].value;
                });
                dayOfWeek.setState(checked);
            }
        });
        daysOfWeekPicker.multiselect('disable');

        timeIntervalFrom.datetimepicker({
            onShow: function (ct) {
                this.setOptions({
                    maxTime: jQuery('#timeIntervalTo').val() ? jQuery('#timeIntervalTo').val() : false
                })
            },
            datepicker: false,
            format: 'H:i'
        });

        timeIntervalTo.datetimepicker({
            onShow: function (ct) {
                this.setOptions({
                    minTime: jQuery('#timeIntervalFrom').val() ? jQuery('#timeIntervalFrom').val() : false
                })
            },
            datepicker: false,
            format: 'H:i'
        });

        dateToUse.datetimepicker({
            timepicker: false,
            format: 'Y-m-d'
        });
    }

    self.deleteFrames = function () {
        if (self.metaFrames().filter(function (metaframe) {
            return !metaframe.selected;
        }).length == 0) {
            alert("Нельзя удалить все фреймы!");
            return;
        }
        self.metaFrames.remove(function (frame) {
            return frame.selected;
        });
    }

    self.addFrame = function () {
        var index = Math.max(...self.metaFrames().map(function (f) {
            return f.index;
        })) + 1;
        var frame = {
            selected: false,
            index: index,
            duration: 5,
            blocks: ko.observableArray(),
            checked: ko.observable(false)
        };
        self.metaFrames.push(frame);
    }

    self.updateSelectedFrame = function () {
        var currentFrame = self.metaFrames().find(function (f) {
            return f.index === self.currentFrame.Index();
        });
        var currentFramePosition = self.metaFrames.indexOf(currentFrame);

        currentFrame.duration = self.currentFrame.Duration();
        currentFrame.useInTimeInterval = self.currentFrame.UseInTimeInerval();
        currentFrame.useFromTime = self.currentFrame.UseFromTime();
        currentFrame.useToTime = self.currentFrame.UseToTime();
        currentFrame.useInDayOfWeek = self.currentFrame.UseInDayOfWeek();
        self.daysOfWeek.forEach(function (dayOfWeek) {
            var name = dayOfWeek.name;
            var capitalizedName = name.charAt(0).toUpperCase() + name.slice(1)
            var observableValue = self.currentFrame['UseIn' + capitalizedName];
            currentFrame['useIn' + capitalizedName] = observableValue();
        });
        currentFrame.useInDate = self.currentFrame.UseInDate();
        currentFrame.dateToUse = self.currentFrame.DateToUse();
        self.metaFrames.splice(currentFramePosition, 1, currentFrame);
    }

    self.selectFrame = function (frame) {
        if (self.currentFrame.Index() !== undefined) {
            self.updateSelectedFrame();
        }
        self.currentFrame.Index(frame.index);
        self.currentFrame.Duration(frame.duration);
        self.currentFrame.UseInTimeInerval(frame.useInTimeInterval);
        self.currentFrame.UseFromTime(frame.useFromTime);
        self.currentFrame.UseToTime(frame.useToTime);
        self.currentFrame.UseInDayOfWeek(frame.useInDayOfWeek);
        self.daysOfWeek.forEach(function (dayOfWeek) {
            var name = dayOfWeek.name;
            var capitalizedName = name.charAt(0).toUpperCase() + name.slice(1)
            var frameValue = frame['useIn' + capitalizedName];
            var observableValue = self.currentFrame['UseIn' + capitalizedName];
            observableValue(frameValue);
            if (frameValue) {
                $('#daysOfWeek').multiselect('select', name, false);
            }
            else {
                $('#daysOfWeek').multiselect('deselect', name, false);
            }
        });
        self.currentFrame.UseInDate(frame.useInDate);
        self.currentFrame.DateToUse(frame.dateToUse);
    }

    self.OnDurableKeyDown = function (ctx, e) {
        if (!((e.keyCode > 95 && e.keyCode < 106)
            || (e.keyCode > 47 && e.keyCode < 58)
            || e.keyCode == 8)) {
            return false;
        }
        return true;
    }
}