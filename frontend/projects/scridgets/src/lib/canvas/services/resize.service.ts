import ResizeObserver from 'resize-observer-polyfill';
import { Injectable, ElementRef, NgZone } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

export interface Size {
  width: number;
  height: number;
}

export class ResizeObservable {

  private obs = new Subject<Size>();
  private target: any;
  private ro: ResizeObserver;
  private oldWidth = -1;
  private oldHeight = -1;
  observable: Observable<Size> = this.obs.pipe(debounceTime(100));

  constructor(elementRef: ElementRef, private ngZone: NgZone) {
    this.target = elementRef.nativeElement;
    this.ro = new ResizeObserver(() => {
      const width = this.target.clientWidth;
      const height = this.target.clientHeight;
      if (width !== this.oldWidth || height !== this.oldHeight) {
        this.oldWidth = width;
        this.oldHeight = height;
        this.ngZone.run(() => {
          this.obs.next({ width, height });
        });
      }
    });
  }

  connect() {
    this.ngZone.runOutsideAngular(() => {
      this.ro.observe(this.target);
    });
  }

  disconnect() {
    this.ro.disconnect();
  }

}

@Injectable({
  providedIn: 'root',
})
export class ResizeService {

  constructor(private ngZone: NgZone) {

  }

  watchForChanges(elementRef: ElementRef) {
    return new ResizeObservable(elementRef, this.ngZone);
  }

}
