import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

// If the font is Google font's VT323 and the height is set to 20px then each character will occupy 8x20 pixels...magic
export const MAGIC_FONT_WIDTH = 8;
export const MAGIC_FONT_HEIGHT = 20;
// For some reason the above places each line about two pixels too high
export const MAGIC_Y_OFFSET = 2;

export interface View {

    width: number;
    height: number;

    set(x: number, y: number, value: string): void;
    setMany(x: number, y: number, value: string, length: number, vertical: boolean): void;
    clear(x: number, y: number): void;
    clearMany(x: number, y: number, length: number, vertical: boolean): void;

}

export class Label {

    private x = 0;
    private y = 0;
    private value = '';

    constructor(private view: View, private vertical: boolean) {

    }

    text(value: string) {
        const amountToClear = this.value.length - value.length;
        if (value.length > 0) {
            this.view.setMany(this.x, this.y, value, value.length, this.vertical);
        }
        if (amountToClear > 0) {
            const offsetX = (this.vertical) ? this.x : this.x + value.length;
            const offsetY = (this.vertical) ? this.y + value.length : this.y;
            this.view.clearMany(offsetX, offsetY, amountToClear, this.vertical);
        }
        return this;
    }

    pos(x: number, y: number) {
        this.view.clearMany(this.x, this.y, this.value.length, this.vertical);
        this.x = x;
        this.y = y;
        this.text(this.value);
        return this;
    }

}

export class View implements View {

    constructor(
        private canvas: Canvas,
        private baseX: number,
        private baseY: number,
        public width: number,
        public height: number
    ) {

    }

    set(x: number, y: number, value: string) {
        const xOff = x + this.baseX;
        const yOff = y + this.baseY;
        this.canvas.set(xOff, yOff, value);
    }

    setMany(x: number, y: number, value: string, length: number, vertical: boolean) {
        if (vertical) {
            for (let yOff = 0; yOff < length; yOff++) {
                this.set(x, y + yOff, value[yOff]);
            }
        } else {
            for (let xOff = 0; xOff < length; xOff++) {
                this.set(x + xOff, y, value[xOff]);
            }
        }
    }

    clear(x: number, y: number) {
        this.set(x, y, '');
    }

    clearMany(x: number, y: number, length: number, vertical: boolean) {
        if (vertical) {
            for (let yOff = 0; yOff < length; yOff++) {
                this.set(x, y + yOff, '');
            }
        } else {
            for (let xOff = 0; xOff < length; xOff++) {
                this.set(x + xOff, y, '');
            }
        }
    }


    childView(xOffset: number, yOffset: number, width: number, height: number) {
        return new View(this.canvas, this.baseX + xOffset, this.baseY + yOffset, width, height);
    }
}

export class Canvas {

    width= 0;
    height = 0;
    tiles: string[] = [];
    changed = new Subject<void>();

    resize(width: number, height: number) {
        if (this.width === width && this.height === height) {
            return;
        }
        const oldWidth = this.width;
        const oldHeight = this.height;
        const size = width * height;
        const newCanvas = Array<string>(size);
        newCanvas.fill('');
        for (let y = 0; y < oldHeight; y++) {
            for (let x = 0; x < oldWidth; x++) {
               newCanvas[y * this.width + x] = this.tiles[y * oldWidth + x];
            }
        }
        this.width = width;
        this.height = height;
        this.tiles = newCanvas;
        this.changed.next();
    }

    set(x: number, y: number, value: string) {
        if (x < 0 || x >= this.width || y < 0 || y >= this.height) {
            return;
        }
        const position = y * this.width + x;
        if (this.tiles[position] !== value) {
            this.tiles[position] = value;
            this.changed.next();
        }
    }

    get(x: number, y: number) {
        if (x < 0 || x >= this.width || y < 0 || y >= this.height) {
            return '';
        }
        const position = y * this.width + x;
        return this.tiles[position];
    }

}

@Injectable()
export class ScCanvasService {
    canvas = new Canvas();

    resize(widthInPx: number, heightInPx: number) {
        const widthTiles = Math.floor(widthInPx / MAGIC_FONT_WIDTH);
        const heightTiles = Math.floor(heightInPx / MAGIC_FONT_HEIGHT);
        this.canvas.resize(widthTiles, heightTiles);
    }

}
