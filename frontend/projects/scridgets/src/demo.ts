import { NgModule, Component, ElementRef } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { BrowserModule } from '@angular/platform-browser';

import { ScCanvasComponent } from './lib/canvas/components/canvas.component';

@Component({
  selector: 'demo-scridgets',
  template: `
    <sc-canvas>
    </sc-canvas>
  `,
  styles: [`
    :host {
      display: block;
      margin: 80px 80px 80px 80px;
      width: 500px;
      height: 500px;
    }
  `]
})
export class DemoComponent {

}

@NgModule({
  declarations: [
    ScCanvasComponent,
    DemoComponent
  ],
  imports: [
    BrowserModule
  ],
  exports: [
    ScCanvasComponent,
    DemoComponent
  ],
  bootstrap: [DemoComponent]
})
export class ScridgetsDemoModule { }

(document as any).fonts.load('20px VT323').then(() => {
  platformBrowserDynamic().bootstrapModule(ScridgetsDemoModule)
  .catch(err => console.error(err));
});
