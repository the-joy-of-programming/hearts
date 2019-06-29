import { Component, ElementRef, ViewChild, OnDestroy, AfterContentInit, QueryList, ContentChildren } from '@angular/core';

import { ScCanvasService, View } from '../services/canvas.service';
import { ResizeService, ResizeObservable, Size } from '../services/resize.service';
import { ScridgetsBase } from './scridgets-base';
import { CanvasRenderer } from '../services/canvas.renderer';
import { Theme } from '../services/theme.service';

@Component({
  selector: 'sc-canvas',
  templateUrl: './canvas.component.html',
  styleUrls: ['./canvas.component.scss'],
  providers: [ ScCanvasService ]
})
export class ScCanvasComponent implements AfterContentInit, OnDestroy {

  @ViewChild('canvas')
  private canvasRef!: ElementRef;
  @ContentChildren(ScridgetsBase)
  private root!: QueryList<ScridgetsBase>;
  private renderer!: CanvasRenderer;
  private resizeObservable: ResizeObservable | null = null;
  private width: number = 0;
  private height: number = 0;
  private first = true;

  constructor(
    private resizeService: ResizeService,
    private canvasService: ScCanvasService,
    private theme: Theme,
    private host: ElementRef
  ) {
  }

  ngAfterContentInit() {
    this.resizeObservable = this.resizeService.watchForChanges(this.host);
    this.resizeObservable.connect();
    this.resizeObservable.observable.subscribe(size => {
      this.onSizeChange(size);
    });
    const context = (this.canvasRef.nativeElement as HTMLCanvasElement).getContext('2d');
    if (!context) {
      throw new Error('This browser does not support canvas 2d rendering.  What year is this?!');
    }
    this.renderer = new CanvasRenderer(context, this.theme);
    this.canvasService.canvas.changed.subscribe(() => {
      console.log('Rerender needed');
      if (this.first) {
        this.first = false;
        new View(this.canvasService.canvas, 0, 0, this.canvasService.canvas.width, this.canvasService.canvas.height).setMany(20, 10, 'Hello World', 11, false);
      }
      this.renderer.render(this.canvasService.canvas, this.width, this.height);
    });
  }

  ngOnDestroy() {
    if (this.resizeObservable !== null) {
      this.resizeObservable.disconnect();
    }
  }

  private onSizeChange(size: Size) {
    this.width = size.width;
    this.height = size.height;
    this.canvasRef.nativeElement.width = this.width;
    this.canvasRef.nativeElement.height = this.height;
    console.log(`Resizing to width=${this.width} height=${this.height}`);
    this.canvasService.resize(this.width, this.height);
  }

}
