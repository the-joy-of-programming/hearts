import { Injectable, QueryList, Input, ViewChildren } from '@angular/core';
import { ScCanvasService, View } from '../services/canvas.service';
import { renderComponent } from '@angular/core/src/render3';

@Injectable()
export abstract class ScridgetsComponent {

    @ViewChildren(ScridgetsComponent)
    protected viewChildren!: QueryList<ScridgetsComponent>;

    @Input()
    x: string | number = 0;
    @Input()
    y: string | number = 0;
    @Input()
    width: string | number = 0;
    @Input()
    height: string | number = 0;
    view: View | null = null;

    constructor(private canvasService: ScCanvasService) {

    }

    abstract renderComponent(): void;

    render(view: View) {
        const x = this.getXInChars(view);
        const y = this.getYInChars(view);
        const width = this.getWidthInChars(view);
        const height = this.getHeightInChars(view);
        this.view = view.childView(x, y, width, height);
        this.viewChildren.forEach(viewChild => {
            viewChild.render(this.view as View);
        });
        this.renderComponent();
    }

    private getXInChars(view: View) {
        return this.valueToChars(this.x, view.width);
    }

    private getYInChars(view: View) {
        return this.valueToChars(this.y, view.height);
    }

    private getWidthInChars(view: View) {
        return this.valueToChars(this.width, view.width);
    }

    private getHeightInChars(view: View) {
        return this.valueToChars(this.height, view.height);
    }

    private valueToChars(value: string | number, max: number) {
        if (typeof value === 'number') {
            return value;
        } else if (typeof value === 'string') {
            if (value.endsWith('%')) {
                return this.charsFromPercentage(value, max);
            } else {
                return Math.round(Number(value));
            }
        } else {
            throw new Error('Value for an x/y property must be a number or a string');
        }
    }

    private charsFromPercentage(value: string, max: number) {
        const percentageNumber = Number(value.substr(0, value.length - 1)) / 100;
        return Math.round(max * percentageNumber);
    }

}
