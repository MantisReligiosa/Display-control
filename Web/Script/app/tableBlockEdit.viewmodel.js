function TableBlockEditViewModel(master) {
    var self = this;

    self.rowTypes = ['header', 'odd', 'even'];

    self.caption = ko.observable();
    self.selectedFonts = ko.observableArray([""]);
    self.selectedFontSizes = ko.observableArray([""]);
    self.selectedFontIndexes = ko.observableArray([""]);
    self.filePath = ko.observable();
    self.encoding = ko.observable(0);

    self.rows = ko.observableArray();
    self.header = ko.observableArray();

    self.rowTypes.forEach(function (rowType) {
        self[rowType + 'BackColor'] = ko.observable();
        self[rowType + 'TextColor'] = ko.observable();
        self[rowType + 'Italic'] = ko.observable(false);
        self[rowType + 'Bold'] = ko.observable(false);
        self[rowType + 'Align'] = ko.observable();
    });

    self.setFont = function (font) {
        self.selectedFonts.removeAll();
        self.selectedFonts.push(font);
    };

    self.setFontSize = function (fontSize) {
        self.selectedFontSizes.removeAll();
        self.selectedFontSizes.push(fontSize);
    };

    self.setFontIndex = function (fontIndex) {
        self.selectedFontIndexes.removeAll();
        self.selectedFontIndexes.push(fontIndex);
    };

    self.initializeControls = function () {

        self.rowTypes.forEach(function (rowType) {
            $('#tableBlock' + capitalize(rowType) + 'BackgroundCP').colorpicker({ format: "rgba" });
            $('#tableBlock' + capitalize(rowType) + 'TextColorCP').colorpicker({ format: "rgba" });
        });
    }

    self.openFileDialog = function () {
        $('#inputFile').on('change', function (e) {
            var tmp = $('#inputFile').val();
            if (tmp == "") {
                return;
            }
            var file = this.files[0];
            var reader = new FileReader();

            reader.onload = (function (theFile) {
                return function (e) {
                    var text = e.target.result;
                    app.request(
                        "POST",
                        "/api/parseCSV",
                        { text: text },
                        function (data) {
                            self.header.removeAll();
                            self.header(data.header);
                            self.rows.removeAll();
                            self.rows(data.rows);
                        }
                    );
                };
            })(file);
            var encoding = self.encoding();
            if (encoding == "0") {
                reader.readAsText(file, 'CP1251');
            } else
                if (encoding == "1") {
                    reader.readAsText(file/*, 'CP1251'*/);
                }
            $('#inputFile').val("");
        }).click();
    }

    self.loadFile = function (evt) {
        debugger;
        var file = self.filePath();
        var reader = new FileReader();

        reader.onload = function (event) {
            debugger;
        };

        reader.readAsText(file);
    }

    function capitalize(string) {
        return string.charAt(0).toUpperCase() + string.slice(1);
    }
}