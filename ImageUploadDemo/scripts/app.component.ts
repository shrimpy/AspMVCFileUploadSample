import {Component, ElementRef, OnInit} from 'angular2/core';
import {FORM_DIRECTIVES, NgIf} from 'angular2/common';

declare var jQuery: any;

@Component({
    selector: 'my-app',
    directives: [FORM_DIRECTIVES, NgIf],
    templateUrl: "tmpl/app.component.html"
})

export class AppComponent implements OnInit {

    public imgSrc: string;
    public message: string;
    public displayError: boolean;

    get diagnostic() { return JSON.stringify(this); }

    ngOnInit() {
        var self = this;
        jQuery("#fileupload").fileupload({
            url: "//" + window.location.host + "/api/upload",
            dataType: 'json',
            acceptFileTypes: /(\.|\/)(jpe?g|png)$/i,
            maxFileSize: 5120001, // 5 MB
            done: function (e, data) {
                if (data && data.response() && data.response().result && data.response().result.url) {
                    self.displayError = false;
                    self.message = "Successfully Uploaded";
                    self.imgSrc = data.response().result.url;
                } else {
                    self.displayError = true;
                    self.message = "Unexpected error happened, please try again.";
                }
            },
            processstart: function (e, data) {
                self.displayError = false;
                self.message = "Uploading ...";
                self.imgSrc = "";
                jQuery("#fileupload").prop("disabled", true);
            },
            processfail: function (e, data) {
                self.displayError = true;
                var error: string = "";
                if (data && data.originalFiles) {
                    data.originalFiles.forEach((item) => {
                        error += item.error + " ";
                    });
                }

                self.message = error + " JPG/PNG only with file no larger than 5MB";
            },
            fail: function (e, data) {
                self.displayError = true;
                if (data && data.errorThrown) {
                    self.message = data.errorThrown;
                }
            },
            always: function () {
                jQuery("#fileupload").prop("disabled", false);
            }
        });
    }
}