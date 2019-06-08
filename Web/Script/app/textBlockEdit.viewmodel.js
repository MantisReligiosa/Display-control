function TextBlockEditViewModel(master) {
	var self = this;

    self.caption = ko.observable();
    self.textColor = ko.observable();
	self.backColor = ko.observable();
	self.selectedFonts = ko.observableArray([""]);
    self.selectedFontSizes = ko.observableArray([""]);
    self.selectedFontIndexes = ko.observableArray([""]);
    self.text = ko.observable();
    self.align = ko.observable(0);
    self.italic = ko.observable(false);
    self.bold = ko.observable(false);
    self.win1251 = ko.observable(true);
    self.utf8 = ko.observable(false);

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
        $('#textBlockBackgroundCP').colorpicker({
            format: "rgba"
        });

        $('#textBlockTextColorCP').colorpicker({
            format: "rgba"
        });
    }
}