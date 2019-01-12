﻿function TextBlockEditViewModel(master) {
	var self = this;

	self.textColor = ko.observable();
	self.backColor = ko.observable();
	self.selectedFonts = ko.observableArray([""]);
    self.selectedFontSizes = ko.observableArray([""]);
    self.text = ko.observable();
    self.align = ko.observable(0);
    self.italic = ko.observable(false);
    self.bold = ko.observable(false);

	self.setFont = function (font) {
		self.selectedFonts.removeAll();
		self.selectedFonts.push(font);
	};

	self.setFontSize = function (fontSize) {
	    self.selectedFontSizes.removeAll();
        self.selectedFontSizes.push(fontSize);
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