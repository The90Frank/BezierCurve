# BezierCurve

An interactive Bezier curve editor written in F#, based on the tutorial by Cisternino (Unipi).

Tutorial playlist: https://www.youtube.com/watch?v=iaKZSFkDZuI&list=PLH0ZF0pFNhGg5fa1g1V6yuoHgkWPCj80D

## Features

- Draw and manipulate Bezier curves through draggable control points
- Add new curve segments by clicking on the canvas
- Apply geometric transformations (translate, rotate, scale)
- Anti-aliased rendering with GDI+

## Controls

| Key | Action |
|---|---|
| W / A / S / D | Translate the curve |
| Q / E | Rotate the curve |
| R / F | Scale the curve |
| NumPad 8/2/4/6 | Translate from window center |
| NumPad 7/9 | Rotate from window center |
| NumPad +/- | Scale from window center |
| Mouse click + drag | Move control points |
| Mouse click on canvas | Add a new curve segment |

## Tech stack

- F# on .NET Framework 4.6.1
- Windows Forms / GDI+

## License

Apache License 2.0 - see [LICENSE](LICENSE) for details.
