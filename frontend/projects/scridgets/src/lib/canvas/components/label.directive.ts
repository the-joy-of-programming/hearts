import { Directive, Input, forwardRef } from '@angular/core';

import { View } from '../services/canvas.service';
import { ScridgetsBase } from './scridgets-base';

@Directive({
  selector: 'sc-label',
  providers: [{provide: ScridgetsBase, useExisting: forwardRef(() => ScLabelDirective)}]
})
export class ScLabelDirective extends ScridgetsBase {

  @Input()
  x!: number;
  @Input()
  y!: number;
  @Input()
  text!: string;

  render(view: View) {
    const coords = this.coordinatesFromInput(this.x, this.y, view);
    view.set(coords.x, coords.y, this.text);
  }

}
