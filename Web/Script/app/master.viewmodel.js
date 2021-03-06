function masterViewModel(app) {
    var self = this,
        clipboard;

    self.fonts = ko.observableArray([]);
    self.fontSizes = ko.observableArray([]);
    self.fontIndexes = ko.observableArray([]);
    self.datetimeformats = ko.observableArray([]);
    self.screenHeight = ko.observable();
    self.screenWidth = ko.observable();
    self.screens = ko.observableArray();
    self.blocks = ko.observableArray();

    self.selectedBlock = ko.observable();
    self.gridSteps = ko.observableArray([5, 10, 20, 25, 50]);
    self.selectedGridSteps = ko.observableArray([5]);
    self.gridEnabled = ko.observable(true);


    self.zoomStep = ko.observable(5);
    self.scales = ko.observableArray([
        { value: 10, label: "10%" },
        { value: 100, label: "100%" },
        { value: 125, label: "125%" }
    ]);
    self.minScale = ko.computed(function () {
        return Math.min.apply(
            Math, $.map(self.scales(), function (val, i) {
                return val.value
            }));
    });
    self.maxScale = ko.computed(function () {
        return Math.max.apply(
            Math, $.map(self.scales(), function (val, i) {
                return val.value
            }));
    });
    self.zoomValue = ko.observable(100);
    self.scale = ko.computed(function () {
        return self.zoomValue() / 100;
    });

    self.textBlockEditViewModel = ko.computed(function () { return new TextBlockEditViewModel(self); });
    self.tableBlockEditViewModel = ko.computed(function () { return new TableBlockEditViewModel(self); });
    self.pictureBlockEditViewModel = ko.computed(function () { return new PictureBlockEditViewModel(self); });
    self.datetimeBlockEditViewModel = ko.computed(function () { return new DatetimeBlockEditViewModel(self); });
    self.positionViewModel = ko.computed(function () { return new PositionViewModel(self); });
    self.backgroundPropertiesMode = ko.observable(true);

    self.background = ko.observable("#ffffff");

    self.zoomFit = function () {
        self.zoomValue(100);
    }

    self.zoomIn = function () {
        var t = +self.zoomValue();
        if (t < self.maxScale()) {
            self.zoomValue(t + self.zoomStep());
        }
    }

    self.zoomOut = function () {
        var t = +self.zoomValue();
        if (t > self.minScale()) {
            self.zoomValue(t - self.zoomStep());
        }
    }

    self.addTextBlock = function () {
        app.request(
            "POST",
            "/api/addTextBlock",
            {},
            function (data) {
                data.selected = false;
                self.blocks.push(data);
            }
        );
    };

    self.addTableBlock = function () {
        app.request(
            "POST",
            "api/addTableBlock",
            {},
            function (data) {
                data.selected = false;
                self.blocks.push(data);
            }
        );
    }

    self.addPictureBlock = function () {
        app.request(
            "POST",
            "api/addPictureBlock",
            {},
            function (data) {
                data.selected = false;
                self.blocks.push(data);
            }
        );
    }

    self.addDateTimeBlock = function () {
        app.request(
            "POST",
            "api/addDateTimeBlock",
            {},
            function (data) {
                data.selected = false;
                data.text = '';
                self.blocks.push(data);
            }
        );
    }

    self.showPosition = function () {
        var block = self.selectedBlock();
        if (block == null) {
            return
        }
        $('#position').modal({ backdrop: 'static', keyboard: false })
            .modal("show");
        self.positionViewModel().top(block.top);
        self.positionViewModel().left(block.left);
        self.positionViewModel().width(block.width);
        self.positionViewModel().height(block.height)
        self.positionViewModel().zIndex(block.zIndex);
    }

    self.applyPosition = function () {
        var block = self.selectedBlock();
        if (block == null) {
            return
        }
        $('#position').modal("hide");
        self.blocks.remove(block);
        block.top = +self.positionViewModel().top();
        block.left = +self.positionViewModel().left();
        block.width = +self.positionViewModel().width();
        block.height = +self.positionViewModel().height()
        block.zIndex = +self.positionViewModel().zIndex();
        app.request(
            "POST",
            "/api/saveBlock",
            block,
            function (data) {
                self.blocks.push(block);
            }
        );
    }

    self.showBackgroundProperties = function () {
        self.backgroundPropertiesMode(true);
        $("#properties")
            .modal({ backdrop: 'static', keyboard: false })
            .modal("show");
    }


    self.showProperties = function () {
        var block = self.selectedBlock();
        if (block == null) {
            return;
        };
        self.backgroundPropertiesMode(false);
        $("#properties")
            .modal({ backdrop: 'static', keyboard: false })
            .modal("show");
        if (block.type === 'text') {
            self.textBlockEditViewModel().backColor(block.backColor);
            self.textBlockEditViewModel().textColor(block.textColor);
            self.textBlockEditViewModel().setFont(block.font);
            self.textBlockEditViewModel().setFontSize(block.fontSize);
            self.textBlockEditViewModel().setFontIndex(block.fontIndex);
            self.textBlockEditViewModel().text(block.text);
            self.textBlockEditViewModel().align(block.align.toString());
            self.textBlockEditViewModel().italic(block.italic);
            self.textBlockEditViewModel().bold(block.bold);
        };
        if (block.type === 'datetime') {
            self.datetimeBlockEditViewModel().backColor(block.backColor);
            self.datetimeBlockEditViewModel().textColor(block.textColor);
            self.datetimeBlockEditViewModel().setFont(block.font);
            self.datetimeBlockEditViewModel().setFontSize(block.fontSize);
            self.datetimeBlockEditViewModel().setFontIndex(block.fontIndex);
            self.datetimeBlockEditViewModel().setFormat(block.format);
            self.datetimeBlockEditViewModel().align(block.align.toString());
            self.datetimeBlockEditViewModel().italic(block.italic);
            self.datetimeBlockEditViewModel().bold(block.bold);
        };
        if (block.type === 'table') {
            self.tableBlockEditViewModel().setFont(block.font);
            self.tableBlockEditViewModel().setFontSize(block.fontSize);
            self.tableBlockEditViewModel().setFontIndex(block.fontIndex);

            self.tableBlockEditViewModel().rowTypes.forEach(function (rowType) {
                self.tableBlockEditViewModel()[rowType + 'TextColor'](block[rowType + 'Style'].textColor);
                self.tableBlockEditViewModel()[rowType + 'BackColor'](block[rowType + 'Style'].backColor);
                self.tableBlockEditViewModel()[rowType + 'Italic'](block[rowType + 'Style'].italic);
                self.tableBlockEditViewModel()[rowType + 'Bold'](block[rowType + 'Style'].bold);
                self.tableBlockEditViewModel()[rowType + 'Align'](block[rowType + 'Style'].align.toString());
            });

            self.tableBlockEditViewModel().rows(block.rows);
            self.tableBlockEditViewModel().header(block.header);
        };
        if (block.type === 'picture') {
            self.pictureBlockEditViewModel().base64Image(block.base64Src);
        };

    };

    self.applyProperties = function () {
        $("#properties").modal("hide");
        var block = self.selectedBlock();
        if (self.backgroundPropertiesMode()) {
            app.request(
                "POST",
                "/api/setBackground",
                { color: backColor },
                function () {
                    self.background(backColor);
                }
            );
            return;
        }
        if (block.type === 'text') {
            self.blocks.remove(block);
            block.backColor = self.textBlockEditViewModel().backColor();
            block.textColor = self.textBlockEditViewModel().textColor();
            block.font = self.textBlockEditViewModel().selectedFonts()[0];
            block.fontSize = self.textBlockEditViewModel().selectedFontSizes()[0];
            block.fontIndex = self.textBlockEditViewModel().selectedFontIndexes()[0];
            block.text = self.textBlockEditViewModel().text();
            block.align = self.textBlockEditViewModel().align();
            block.italic = self.textBlockEditViewModel().italic();
            block.bold = self.textBlockEditViewModel().bold();
        };
        if (block.type === 'datetime') {
            self.blocks.remove(block);
            block.backColor = self.datetimeBlockEditViewModel().backColor();
            block.textColor = self.datetimeBlockEditViewModel().textColor();
            block.font = self.datetimeBlockEditViewModel().selectedFonts()[0];
            block.fontSize = self.datetimeBlockEditViewModel().selectedFontSizes()[0];
            block.fontIndex = self.datetimeBlockEditViewModel().selectedFontIndexes()[0];
            block.format = self.datetimeBlockEditViewModel().selectedFormats()[0];
            block.align = self.datetimeBlockEditViewModel().align();
            block.italic = self.datetimeBlockEditViewModel().italic();
            block.bold = self.datetimeBlockEditViewModel().bold();
        };
        if (block.type === 'table') {
            self.blocks.remove(block);
            block.font = self.tableBlockEditViewModel().selectedFonts()[0];
            block.fontSize = self.tableBlockEditViewModel().selectedFontSizes()[0];
            block.fontIndex = self.tableBlockEditViewModel().selectedFontIndexes()[0];
            self.tableBlockEditViewModel().rowTypes.forEach(function (rowType) {
                block[rowType + 'Style'].textColor = self.tableBlockEditViewModel()[rowType + 'TextColor']();
                block[rowType + 'Style'].backColor = self.tableBlockEditViewModel()[rowType + 'BackColor']();
                block[rowType + 'Style'].italic = self.tableBlockEditViewModel()[rowType + 'Italic']();
                block[rowType + 'Style'].bold = self.tableBlockEditViewModel()[rowType + 'Bold']();
                block[rowType + 'Style'].align = self.tableBlockEditViewModel()[rowType + 'Align']();
            });
            block.rows = self.tableBlockEditViewModel().rows();
            block.header = self.tableBlockEditViewModel().header();
        }
        if (block.type === 'picture') {
            self.blocks.remove(block);
            block.base64Src = self.pictureBlockEditViewModel().base64Image();
        }
        app.request(
            "POST",
            "/api/saveBlock",
            block,
            function (data) {
                self.blocks.push(block);
            }
        );
    };

    var isCopying = false;

    self.copy = function () {
        isCopying = true;
        clipboard = self.selectedBlock();
    };

    self.cut = function () {
        isCopying = false;
        clipboard = self.selectedBlock();
        self.blocks.remove(clipboard);
    };

    self.paste = function () {
        if (clipboard == null) {
            return;
        }
        app.request(
            "POST",
            "/api/copyBlock",
            clipboard,
            function (data) {
                data.selected = true;
                self.selectedBlock(data);
                self.blocks.push(data);
                if (!isCopying) {
                    app.request(
                        "POST",
                        "/api/deleteBlock",
                        clipboard,
                        function (data) { }
                    );
                }
                else {
                    self.blocks.remove(clipboard);
                    clipboard.selected = false;
                    self.blocks.push(clipboard);
                }
            }
        );
    };

    self.deleteBlock = function () {
        var block = self.selectedBlock();
        if (block == null) {
            return;
        }
        app.request(
            "POST",
            "/api/deleteBlock",
            block,
            function (data) {
                self.blocks.remove(block);
                self.selectedBlock(null);
            }
        );
    };

    self.zUp = function () {
        if (self.selectedBlock() == null) {
            return;
        }
        var block = self.selectedBlock();
        self.blocks.remove(block);
        block.zIndex++;
        app.request(
            "POST",
            "/api/saveBlock",
            block,
            function (data) {
                self.blocks.push(block);
            }
        );
    };

    self.zDown = function () {
        if (self.selectedBlock() == null) {
            return;
        }
        var block = self.selectedBlock();
        if (block.zIndex == 0) {
            app.infoMsg("z-index is minimal");
            return;
        }
        self.blocks.remove(block);
        block.zIndex--;
        app.request(
            "POST",
            "/api/saveBlock",
            block,
            function (data) {
                self.blocks.push(block);
            }
        );

    };

    self.downloadConfig = function () {
        window.location = "/api/downloadConfig";
    }

    self.uploadConfig = function () {
        $('<input>')
            .attr('type', 'file')
            .attr('accept', '.xml')
            .on('change', function (e) {
                var file = this.files[0];
                var reader = new FileReader();

                reader.onload = (function (theFile) {
                    return function (e) {
                        var text = e.target.result;
                        app.request(
                            "POST",
                            "/api/uploadConfig",
                            { text: text },
                            function (data) {
                                loadBackground()
                                    .then(function () { return loadBlocks(); });
                            }
                        );
                    };
                })(file);

                reader.readAsText(file/*, 'CP1251'*/);
            })
            .click();
    }

    selectBlock = function (bind) {
        self.blocks.remove(bind);
        unselectBlocks();
        self.selectedBlock.selected = false;
        bind.selected = true;
        self.blocks.push(bind);
        self.selectedBlock(bind);
        initializeControls();
    };

    unselectBlocks = function () {
        var blocks = self.blocks.remove(function (block) { return block.selected; });
        blocks.forEach(function (block) {
            block.selected = false;
            self.blocks.push(block);
        });
        self.selectedBlock(null);
        initializeControls();
    }

    $(document).ready(function () {
        initializeControls();
        loadFonts()
            .then(function () { return loadResolution(); })
            .then(function () { return loadDatetimeFormats(); })
            .then(function () { return loadBackground(); })
            .then(function () { return loadBlocks(); });
        initReact();
    });

    var backColor;

    initializeControls = function () {
        $('#backgroundCP').colorpicker({
            format: "rgba"
        });
        $('#backgroundCP').on('colorpickerChange', function (e) {
            backColor = e.color.toString();
        });

        self.textBlockEditViewModel().initializeControls();
        self.tableBlockEditViewModel().initializeControls();
        self.datetimeBlockEditViewModel().initializeControls();
    };

    initReact = function () {
        interact('.resize-drag')
            .draggable({
                inertia: true,
                restrict: {
                    restriction: '.workspace',
                    endOnly: true,
                    elementRect: { top: 0, left: 0, bottom: 1, right: 1 }
                },
                autoScroll: true,

                onmove: onResizeMove,

                onend: applyResizeMove
            })
            .resizable({
                // resize from all edges and corners
                edges: { left: true, right: true, bottom: true, top: true },

                // keep the edges inside the parent
                restrictEdges: {
                    outer: '.workspace',
                    endOnly: true,
                },

                // minimum size
                restrictSize: {
                    min: { width: 100, height: 50 },
                },

                inertia: true,
            })
            .on('resizemove', onResizeMove)
            .on('resizeend', applyResizeMove);
    };

    onResizeMove = function (event) {
        var target = event.target,
            // keep the dragged position in the data-x/data-y attributes
            scale = self.scale();
        x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx / scale,
            y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy / scale;

        if (event.rect != null) {
            x = (parseFloat(target.getAttribute('data-x')) || 0);
            y = (parseFloat(target.getAttribute('data-y')) || 0);

            // update the element's style
            target.style.width = event.rect.width / scale + 'px';
            target.style.height = event.rect.height / scale + 'px';

            // translate when resizing from top or left edges
            x += event.deltaRect.left / scale;
            y += event.deltaRect.top / scale;

            target.setAttribute('data-w', event.rect.width / scale);
            target.setAttribute('data-h', event.rect.height / scale);
        }

        target.style.webkitTransform =
            target.style.transform =
            'translate(' + x + 'px, ' + y + 'px)';

        // update the posiion attributes
        target.setAttribute('data-x', x);
        target.setAttribute('data-y', y);
    };

    applyResizeMove = function (event) {
        var target = event.target;
        var id = target.getAttribute('id');
        var block = self.blocks.remove(function (block) { return block.id === id; })[0];
        var w = +target.getAttribute('data-w');
        var h = +target.getAttribute('data-h');
        if (w == 0) {
            w = block.width;
        }
        if (h == 0) {
            h = block.height;
        }
        if (self.gridEnabled()) {
            w = adjustToStep(w);
            h = adjustToStep(h);
        }

        var x = +target.getAttribute('data-x') + block.left;
        var y = +target.getAttribute('data-y') + block.top;
        var screen = self.screens().find(function (screen) {
            return pointInScreen(screen, x, y)
                && pointInScreen(screen, x + w, y)
                && pointInScreen(screen, x, y + h)
                && pointInScreen(screen, x + w, y + h);
        });

        var isInScreens = (screen != null);
        if (!isInScreens) {
            var screenLeft = self.screens().find(function (screen) {
                return pointInScreen(screen, x, y)
                    && pointInScreen(screen, x, y + h);
            });
            var screenRight = self.screens().find(function (screen) {
                return pointInScreen(screen, x + w, y)
                    && pointInScreen(screen, x + w, y + h);
            });
            isInScreens = (screenLeft != null) && (screenRight != null);
            screen = screenLeft;
        }

        if (!isInScreens) {
            var screenTop = self.screens().find(function (screen) {
                return pointInScreen(screen, x, y)
                    && pointInScreen(screen, x + w, y);
            });
            var screenBottom = self.screens().find(function (screen) {
                return pointInScreen(screen, x, y + h)
                    && pointInScreen(screen, x + w, y + h);
            });
            isInScreens = (screenTop != null) && (screenBottom != null);
            screen = screenTop;
        }

        if (isInScreens) {
            if (self.gridEnabled()) {

                var deltaX = x - screen.left;
                var deltaY = y - screen.top;
                x = screen.left + adjustToStep(deltaX);
                y = screen.top + adjustToStep(deltaY);
            };
            block.width = w;
            block.height = h;
            block.left = x;
            block.top = y;
        }
        self.blocks.push(block);
        selectBlock(block);
        resizeAndMoveBlock(block);
    };

    pointInScreen = function (screen, x, y) {
        return screen.left <= x && screen.left + screen.width >= x && screen.top <= y && screen.top + screen.height >= y;
    }

    resizeAndMoveBlock = function (block) {
        app.request(
            "POST",
            "/api/moveAndResize",
            {
                Id: block.id,
                Height: block.height,
                Width: block.width,
                Left: block.left,
                Top: block.top
            },
            function (data) { }
        );
    }

    adjustToStep = function (value) {
        var step = self.selectedGridSteps()[0];
        return Math.round(value / step) * step;
    }

    saveBlock = function (block) {
        app.request(
            "POST",
            "/api/saveBlock",
            block,
            function (data) { }
        );
    };

    var timer = setInterval(function () {
        updateDatetimeBlocksValues();
    }, 100);

    updateDatetimeBlocksValues = function () {
        var datetimeblocks = $(".datetimeblock");
        datetimeblocks.each(function (index) {
            var datetimeFormat = this.getAttribute('datetimeformat');
            var datetime = moment().format(datetimeFormat);
            $(this).text(datetime);
        });
    }

    loadBlocks = function () {
        app.request(
            "GET",
            "/api/blocks",
            {},
            function (data) {
                data.forEach(function (block) {
                    block.selected = false;
                    if (block.type = 'datetime') {
                        block.text = ''
                        block.format = (block.format == undefined) ? null : block.format
                    }
                    self.blocks.push(block);
                });

            }
        );
    }

    loadResolution = function () {
        return new Promise(
            function (resolve, reject) {
                app.request(
                    "POST",
                    "/api/screenResolution",
                    {
                        refreshData: false
                    },
                    function (data) {
                        self.screenHeight(data.height);
                        self.screenWidth(data.width);
                        data.displays.forEach(function (screen) {
                            self.screens.push(screen);
                        });
                        resolve();
                    });
            });
    }

    resetDisplays = function () {
        app.request(
            "POST",
            "/api/screenResolution",
            {
                refreshData: true
            },
            function (data) {
                self.screenHeight(data.height);
                self.screenWidth(data.width);
                self.screens.removeAll();
                data.displays.forEach(function (screen) {
                    self.screens.push(screen);
                });
            });
    }

    loadBackground = function () {
        return new Promise(
            function (resolve, reject) {
                app.request(
                    "GET",
                    "/api/background",
                    {},
                    function (data) {
                        if (data == '') {
                            self.background("#ffffff");
                        }
                        else {
                            self.background(data);
                        }
                        resolve();
                    });
            });
    }

    loadFonts = function () {
        return new Promise(
            function (resolve, reject) {
                app.request("POST", "/api/fonts", {}, function (data) {
                    data.fonts.forEach(function (entry) {
                        self.fonts.push(entry);
                    });
                    data.sizes.forEach(function (entry) {
                        self.fontSizes.push(entry);
                    });
                    data.indexes.forEach(function (entry) {
                        self.fontIndexes.push(entry);
                    });
                    resolve();
                });
            });
    }

    loadDatetimeFormats = function () {
        return new Promise(
            function (resolve, reject) {
                app.request("POST", "/api/datetimeformats", {}, function (data) {
                    data.forEach(function (entry) {
                        self.datetimeformats.push(entry);
                    });
                    resolve();
                });
            });
    }

    self.startShow = function () {
        app.request(
            "POST",
            "/api/startShow",
            {},
            function (data) { }
        );
    }

    self.stopShow = function () {
        app.request(
            "POST",
            "/api/stopShow",
            {},
            function (data) { }
        );
    }
}

app.attach({
    sourceName: 'master',
    factory: masterViewModel
});