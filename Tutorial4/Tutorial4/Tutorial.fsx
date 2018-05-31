open System.Windows.Forms
open System.Drawing
open System.Drawing.Drawing2D

type Painting() as this = 
    inherit UserControl()

    let mutable lined = [|PointF(150.f,150.f); PointF(100.f,150.f); PointF(100.f,50.f); PointF(50.f,50.f)|]
    let mutable matrix = [|new Drawing2D.Matrix(); new Drawing2D.Matrix()|]
    let mutable R = 6.f
    let Rpen = new Pen(Color.Red, R/6.f)
    let Lpen = new Pen(Color.Black, 3.f/(R/6.f))
    let Cpen = new Pen(Color.Green, 2.f*R/6.f)
    let Cbrush = Brushes.Green
    let Apen = new Pen(Color.Blue, 0.5f*R/6.f)
    let mutable drag = None

    let handleRect(p:PointF) (r:single) =
        RectangleF(PointF(p.X-r,p.Y-r),SizeF(2.f*r,2.f*r))

    let isInHandle (p:PointF) (x:float32) (y:float32) r=
        let sqr v = v*v
        let x1 ,y1 = single x - p.X, single y - p.Y 
        sqr x1 + sqr y1 <= sqr r

    let drawCircle (pl:PointF[]) (gr:Graphics) = 
        for i in 0 .. ( (pl.Length) - 1) do
            if (i%3 = 0) then
                gr.DrawEllipse(Rpen, (handleRect pl.[i] R))
            else
                gr.FillEllipse(Cbrush, (handleRect pl.[i] R))
        done

    do 
        this.SetStyle(ControlStyles.DoubleBuffer, true)
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true)
    done

    member this.NormalTranslate x y =
        do
            matrix.[0].Translate(x,y)
            matrix.[1].Translate(-x,-y,MatrixOrder.Append)
            this.Invalidate()
        done
    
    member this.NormalScale x y =
        do
            matrix.[0].Scale(x,y)
            matrix.[1].Scale(1.f/x,1.f/y,MatrixOrder.Append)
            this.Invalidate()
        done

    member this.NormalRotate a =
        do
            matrix.[0].Rotate(a)
            matrix.[1].Rotate(-a,MatrixOrder.Append)
            this.Invalidate()
        done
    
    member this.CenterWindowsRotate a =
        do
            let center = [|PointF(single this.ClientSize.Width/2.f,single this.ClientSize.Height/2.f)|]
            matrix.[1].TransformPoints(center)
            let xc, yc = center.[0].X, center.[0].Y
            
            matrix.[0].Translate(xc,yc)
            matrix.[1].Translate(-xc,-yc,MatrixOrder.Append)

            matrix.[0].Rotate(a)
            matrix.[1].Rotate(-a,MatrixOrder.Append)
            
            matrix.[0].Translate(-xc,-yc)
            matrix.[1].Translate(xc,yc,MatrixOrder.Append)
            this.Invalidate()
        done

    member this.CenterWindowsScale x y =
        do
            let center = [|PointF(single this.ClientSize.Width/2.f,single this.ClientSize.Height/2.f)|]
            matrix.[1].TransformPoints(center)
            let xc, yc = center.[0].X, center.[0].Y
            
            matrix.[0].Translate(xc,yc)
            matrix.[1].Translate(-xc,-yc,MatrixOrder.Append)

            matrix.[0].Scale(x,y)
            matrix.[1].Scale(1.f/x,1.f/y,MatrixOrder.Append)

            matrix.[0].Translate(-xc,-yc)
            matrix.[1].Translate(xc,yc,MatrixOrder.Append)

            this.Invalidate()
        done

    member this.CenterWindowsTranslate x y =
        do
            matrix.[0].Translate(x,y,MatrixOrder.Append)
            matrix.[1].Translate(-x,-y,MatrixOrder.Prepend)
            this.Invalidate()
        done

    override this.OnPaint e =
        let g = e.Graphics
        g.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        g.Transform <- matrix.[0]
        g.DrawLine(Apen, PointF(0.f,0.f), PointF(15.f,0.f))
        g.DrawLine(Apen, PointF(0.f,0.f), PointF(0.f,15.f))
        let size = lined.Length - 1
        for i in 0 .. ((size/3)-1) do
            let j = 3*i
            g.DrawLine(Cpen, lined.[j], lined.[(j+1)])
            g.DrawLine(Cpen, lined.[j+2], lined.[(j+3)])
        done
        drawCircle lined g
        g.DrawBeziers(Lpen, lined)
        g.DrawString("0",this.Font,Brushes.Blue,PointF(-10.f,-10.f))
        

    override this.OnResize _ = 
        this.Invalidate()

    override this.OnMouseDown e =
        let mutable selected = [|PointF(single e.X, single e.Y)|]
        matrix.[1].TransformPoints(selected)
        match (lined |> Array.tryFindIndex(fun p -> isInHandle p selected.[0].X selected.[0].Y R)) with
            | Some idx -> do (drag <- Some (idx, selected.[0].X-lined.[idx].X, selected.[0].Y-lined.[idx].Y)) done
            | _ -> do
                let middlepoint1 = PointF((2.f*selected.[0].X + lined.[0].X)/3.f,(2.f*selected.[0].Y + lined.[0].Y)/3.f)
                let middlepoint2 = PointF((selected.[0].X + 2.f*lined.[0].X)/3.f,(selected.[0].Y + 2.f*lined.[0].Y)/3.f)
                selected <- Array.append selected [| middlepoint1; middlepoint2|]
                lined <- Array.append selected lined
        this.Invalidate()
        
    override this.OnMouseUp e =
        do drag <- None done

    override this.OnMouseMove e =
        let mutable selected = [|PointF(single e.X, single e.Y)|]
        matrix.[1].TransformPoints(selected)
        match drag with
        | Some (idx,pX,pY) -> do 
            lined.[idx] <- PointF(single selected.[0].X-pX,single selected.[0].Y-pY)
            this.Invalidate() done
        | _ -> do () done

    override this.OnKeyDown e =
        match e.KeyCode with
        | Keys.W -> this.NormalTranslate 0.f -10.f
        | Keys.S -> this.NormalTranslate 0.f 10.f
        | Keys.A -> this.NormalTranslate -10.f 0.f
        | Keys.D -> this.NormalTranslate 10.f 0.f
        | Keys.Q -> this.NormalRotate 5.f
        | Keys.E -> this.NormalRotate -5.f
        | Keys.R -> this.NormalScale 2.f 2.f
        | Keys.F -> this.NormalScale 0.5f 0.5f
        | Keys.NumPad7 -> this.CenterWindowsRotate 5.f
        | Keys.NumPad9 -> this.CenterWindowsRotate -5.f
        | Keys.NumPad8 -> this.CenterWindowsTranslate 0.f -10.f
        | Keys.NumPad2 -> this.CenterWindowsTranslate 0.f 10.f
        | Keys.NumPad4 -> this.CenterWindowsTranslate -10.f 0.f
        | Keys.NumPad6 -> this.CenterWindowsTranslate 10.f 0.f
        | Keys.Add -> this.CenterWindowsScale 2.f 2.f
        | Keys.Subtract -> this.CenterWindowsScale 0.5f 0.5f
        | _ -> ()

let f = new Form(Text = "DrawLine")
f.Show()

let p = new Painting(Dock = DockStyle.Fill)
f.Controls.Add(p)
p.Select()

#if COMPILED
module BoilerPlateForForm = 
    [<System.STAThread>]
    do ()
    do System.Windows.Forms.Application.Run()
#endif

