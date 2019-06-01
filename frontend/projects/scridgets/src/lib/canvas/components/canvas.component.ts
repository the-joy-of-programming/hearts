import { Component, OnInit, ElementRef, ViewChild, OnDestroy } from '@angular/core';

import { ScCanvasService } from '../services/canvas.service';
import { ResizeService, ResizeObservable } from '../services/resize.service';
import { ScridgetsComponent } from './scridgets-component';

@Component({
  selector: 'sc-canvas',
  templateUrl: './canvas.component.html',
  styleUrls: ['./canvas.component.scss'],
  providers: [ ScCanvasService ]
})
export class ScCanvasComponent implements OnInit, OnDestroy {

  @ViewChild('svg')
  private svg!: ElementRef;
  @ViewChild(ScridgetsComponent)
  private root!: ScridgetsComponent;
  private resizeObservable: ResizeObservable | null = null;

  private oldWidth = -1;
  private oldHeight = -1;

  constructor(
    private resizeService: ResizeService,
    private canvasService: ScCanvasService
  ) {

  }

  ngOnInit() {
    this.resizeObservable = this.resizeService.watchForChanges(this.svg);
    this.resizeObservable.connect();
    this.resizeObservable.observable.subscribe(() => {
      this.onSizeChange();
    });
  }

  ngOnDestroy() {
    if (this.resizeObservable !== null) {
      this.resizeObservable.disconnect();
    }
  }

  private onSizeChange() {
    const widthInPx = this.svg.nativeElement.childWidth;
    const heightInPx = this.svg.nativeElement.childHeight;
    if (widthInPx !== this.oldWidth || heightInPx !== this.oldHeight) {
      this.oldWidth = widthInPx;
      this.oldHeight = heightInPx;
      this.svg.nativeElement.setAttribute('width', widthInPx);
      this.svg.nativeElement.setAttribute('height', heightInPx);
      const rootView = this.canvasService.rootView(this.svg.nativeElement as SVGElement, widthInPx, heightInPx);
      this.root.render(rootView);
    }
  }

}
