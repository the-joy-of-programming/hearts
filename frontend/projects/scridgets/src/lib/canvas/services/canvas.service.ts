// If the font is Google font's VT323 and the height is set to 20px then each character will occupy 8x20 pixels...magic
export const MAGIC_FONT_WIDTH = 8;
export const MAGIC_FONT_HEIGHT = 20;
// For some reason the above places each line about two pixels too high
export const MAGIC_Y_OFFSET = 2;

export class TextView {

    constructor(private elem: SVGTextElement) {

    }

    remove() {
        this.elem.remove();
    }

    text(value: string) {
        this.elem.textContent = value;
    }

}

export class View {

    constructor(
        private svg: SVGElement,
        private baseX: number,
        private baseY: number,
        public width: number,
        public height: number
    ) {

    }

    x(xOffsetInCharacters: number) {
        return this.baseX + (xOffsetInCharacters * MAGIC_FONT_WIDTH);
    }

    y(yOffsetInCharacters: number) {
        return this.baseY + (yOffsetInCharacters * MAGIC_FONT_WIDTH);
    }

    calcWidth(numCharacters: number) {
        return numCharacters * MAGIC_FONT_WIDTH;
    }

    calcHeight(numCharacters: number) {
        return numCharacters * MAGIC_FONT_HEIGHT;
    }

    text(charX: number, charY: number) {
        const x = this.x(charX);
        const y = this.y(charY);
        const text = document.createElement('text') as unknown as SVGTextElement;
        text.setAttribute('x', x.toString());
        text.setAttribute('y', y.toString());
        this.svg.appendChild(text);
        return new TextView(text);
    }

    childView(xOffsetInCharacters: number, yOffsetInCharacters: number, width: number, height: number) {
        const baseX = this.x(xOffsetInCharacters);
        const baseY = this.y(yOffsetInCharacters);
        return new View(this.svg, baseX, baseY, width, height);
    }
}

export class ScCanvasService {

    widthPx = 0;
    heightPx  = 0;
    widthTiles = 0;
    heightTiles = 0;

    resize(widthInPx: number, heightInPx: number) {
        this.widthPx = widthInPx;
        this.heightPx = heightInPx;
        this.widthTiles = this.widthPx / MAGIC_FONT_WIDTH;
        this.heightTiles = this.heightPx / MAGIC_FONT_HEIGHT;
    }

    rootView(svg: SVGElement, widthInPx: number, heightInPx: number) {
        const widthInChars = Math.floor(widthInPx);
        const heightInChars = Math.floor(heightInPx);
        return new View(svg, 0, 0, widthInChars, heightInChars);
    }

}
