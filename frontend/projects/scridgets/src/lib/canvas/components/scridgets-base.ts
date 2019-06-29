import { View } from '../services/canvas.service';

export interface Coordinates {
    x: number;
    y: number;
}

export abstract class ScridgetsBase {

    constructor() {

    }

    abstract render(view: View): void;

    protected coordinatesFromInput(x: number | string, y: number | string, view: View) {
        const xVal = this.handlePercentages(x, view.width);
        const yVal = this.handlePercentages(y, view.height);
        return { x: xVal, y: yVal };
    }

    private handlePercentages(value: number | string, max: number) {
        if (typeof value === 'number') {
            return value;
        }
        // Hack off the % and convert to a number
        const percentage = Number(value.substr(0, value.length - 1));
        return Math.round((percentage / 100) * max);
    }

}
