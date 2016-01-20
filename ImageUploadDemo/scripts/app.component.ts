import {Component, ElementRef, OnInit} from 'angular2/core';
import {FORM_DIRECTIVES} from 'angular2/common';

declare var jQuery: any;

@Component({
    selector: 'my-app',
    directives: [FORM_DIRECTIVES],
    templateUrl: "tmpl/app.component.html"
})

export class AppComponent implements OnInit {

    ngOnInit() {
        jQuery("#fileupload").fileupload({
            url: "//" + window.location.host + "/api/upload",
            dataType: 'json',
            acceptFileTypes: /(\.|\/)(jpe?g|png)$/i,
            done: function (e, data) {
                console.log("done: " + data.response().result.url);
            },
            processstart: function () {
                console.log("start");
            },
            processfail: function (e, data) {
                console.log("process fail");
            },
            fail: function (e, data) {
                console.log("fail");
            },
            always: function () {
                console.log("always");
            }
        });
    }
}