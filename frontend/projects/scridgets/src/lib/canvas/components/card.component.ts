import { Component } from '@angular/core';
import { ScridgetsComponent } from './scridgets-component';

@Component({
    selector: 'sc-canvas',
    templateUrl: './canvas.component.html',
    styleUrls: ['./canvas.component.scss'],
    providers: [ ]
})
export class ScCardComponent extends ScridgetsComponent {

    topBar: string;
    bottomBar: string;
    leftBar: string;
    rightBar: string;

    renderComponent() {

    }

}
