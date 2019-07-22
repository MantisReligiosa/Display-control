function PictureBlockEditViewModel(master) {
    var self = this;

    self.caption = ko.observable();
    self.base64Image = ko.observable();

    self.openFileDialog = function () {
        $('#inputPicture').on('change', function (e) {
            var tmp = $('#inputPicture').val();
            if (tmp == "") {
                return;
            }

            var file = this.files[0];
            var reader = new FileReader();

            reader.onload = (function (theFile) {
                return function (e) {
                    var res = e.target.result;
                    //var base64String = btoa(String.fromCharCode.apply(null, new Uint8Array(res)));
                    var base64String = btoa(
                        new Uint8Array(res)
                            .reduce((data, byte) => data + String.fromCharCode(byte), '')
                    );
                    self.base64Image(base64String);
                };
            })(file);

            reader.readAsArrayBuffer(file);
            $('#inputPicture').val("");
        }).click();
    }
}