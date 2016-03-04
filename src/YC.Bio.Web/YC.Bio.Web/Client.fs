namespace YC.Bio.Web

open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client

module wsfc = WebSharper.Formlets.Controls
module wsfe = WebSharper.Formlets.Enhance
module wsfd = WebSharper.Formlets.Data
module wsff = WebSharper.Formlets.Formlet

[<JavaScript>]
module Client =

    let Start input k l h =
        async {
            let! data = Server.DoSomething input l h
            return k data
        }
        |> Async.Start

    let vBox elems = 
        Table[(TR [Attr.VAlign "top"]) -< (List.map (fun x -> TD [x]) elems)]

    let input width = 
        Input [Attr.Value ""; Attr.Style (sprintf "width:%ipx" width); Attr.ReadOnly ""] -< []    
        
    let frm =
        let length =
            wsff.Do {                
                let! x = wsfc.Input "60" |>  wsfd.Validator.IsInt "In" |> wsfe.WithTextLabel "Length limit:" |> wsfe.WithValidationIcon
                let! y =  wsfc.Input "100" |>  wsfd.Validator.IsInt "" |> wsfe.WithTextLabel "-" |> wsfe.WithValidationIcon
                return (int x,int y)
            }            
            |> wsff.Horizontal
            

        wsff.Do {                
                let! x = 
                    let f = wsfc.TextArea "" |> wsfe.WithTextLabel "Input" |> wsfe.WithLabelAbove
                    f
                let! y = length                
                return (x, y)
            }          
        |> wsfe.WithSubmitButton        

    let Main () =
        
        let lengthSpec,getter = 
            let lowIn = input 50
            let highIn = input 50
            lowIn.Value <- "60"
            highIn.Value <- "100"
            let control =
                frm
//                Div [
//                    vBox [ Div [Text "Length limit: "]
//                           lowIn
//                           Div [Text "-"]
//                           highIn
//                         ]
//                ] -< [Attr.Style "width:100%"]
//            control, (fun () -> lowIn.Value |> int, highIn.Value |> int)
            control.Run(fun (l,h) -> ()),(fun () -> lowIn.Value |> int, highIn.Value |> int)
            
        let output = H1 []
        let str = TextArea [Attr.Style "width:670px; height:520px"]
        let grammar =
            TextArea [Attr.Style "width:400px; height:520px"; Attr.ReadOnly ""] 
            -< [Text <| 
                         "[<Start>]\n"
                       + "full: any+ folded\n"
                       + "\n"
                       + "folded: \n"
                       + "   stem<(\n"
                       + "         any*[1..3] \n"
                       + "         stem<any*[7..10]> \n"
                       + "         any*[2..3] \n"
                       + "         stem<any*[5..7]> \n"
                       + "         any*[3..5] \n"
                       + "         stem<any*[5..7]>\n" 
                       + "         any*[0..2]\n"
                       + "        )>\n"
                       + "\n"
                       + "stem<s>: \n"
                       + "      A stem<s> U\n"
                       + "    | U stem<s> A\n"
                       + "    | C stem<s> G\n"
                       + "    | G stem<s> C\n"
                       + "    | G stem<s> U\n"
                       + "    | U stem<s> G\n"
                       + "    | s\n"
                       + "\n"
                       + "any: A | U | G | C\n"

            ]
        let inputFrame = 
            Div[
//                Div [Text "Input:"]            
//                str
                lengthSpec
//                Button [Text "Send"]
//                |>! OnClick (fun _ _ ->
//                    async {
//                        let l, h = getter()
//                        let! data = Server.DoSomething str.Value l h
//                        output.Text <- data
//                    }
//                    |> Async.Start
//                )
                ]-< [Attr.Style "width:100%"]
        
        Div [
            vBox [inputFrame; Div[Div[Text "Grammar:"];grammar]] -< [Attr.Style "width:100%"]
            HR []
            H4 [Attr.Class "text-muted"] -< [Text "The server responded:"]
            Div [Attr.Class "jumbotron"] -< [output]
        ]
