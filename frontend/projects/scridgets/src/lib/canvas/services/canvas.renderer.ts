import { Canvas, MAGIC_FONT_WIDTH, MAGIC_FONT_HEIGHT } from './canvas.service';
import { Theme } from './theme.service';

export class CanvasRenderer {

    constructor(private ctx: CanvasRenderingContext2D, private theme: Theme) {

    }

    render(canvas: Canvas, width: number, height: number) {
        this.ctx.fillStyle = this.theme.backgroundColor;
        this.ctx.fillRect(0, 0, width, height);
        this.ctx.font = '20px VT323'
        this.ctx.strokeStyle = this.theme.foregroundColor;
        this.ctx.fillStyle = this.theme.foregroundColor;
        console.log(`Looping through canvas array with ${canvas.tiles.length} tiles`);
        for (let y = 0; y < canvas.height; y++) {
            for (let x = 0; x < canvas.width; x++) {
                const value = canvas.get(x, y);
                if (value.length > 0) {
                    const xPx = x * MAGIC_FONT_WIDTH;
                    const yPx = y * MAGIC_FONT_HEIGHT;
                    console.log(`Value is not empty: ${value}.  Stroking at ${xPx},${yPx}`);
                    this.ctx.fillText(value, xPx, yPx);
                }
            }
        }
    }

}