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

    let text = 
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


    let frm =
        let length =
            wsff.Do {                
                let! x = wsfc.Input "60" |> wsfe.WithTextLabel "Length limit:" //|>  wsfd.Validator.IsInt "In"  |> wsfe.WithValidationIcon
                let! y =  wsfc.Input "100" |> wsfe.WithTextLabel "-" //|>  wsfd.Validator.IsInt ""  |> wsfe.WithValidationIcon
                return (int x, int y)
            }            
            |> wsff.Horizontal
            

        let left =
            wsff.Do {                
                let! x = wsfc.TextArea "" |> wsfe.WithTextLabel "Input" |> wsfe.WithLabelAbove |> wsfe.WithFormContainer
                let! y = length
                return (x, y)
            }  
            |> wsff.Vertical
            |> wsfe.WithFormContainer
                    
        let right = wsfc.TextArea text |> wsfe.WithTextLabel "Grammar" |> wsfe.WithLabelAbove |> wsfe.WithFormContainer
        
        wsff.Do {                
            let! x = left
            let! y = right                                             
            return (x, y)
            }
        |> wsff.Horizontal
        |> wsfe.WithSubmitButton        
        |> wsfe.WithFormContainer
     

    let Main () =

        let output = H1 []
        let lengthSpec =
            frm.Run(fun ((inp,(l,h)),g) -> 
            async {                        
                    let! data = Server.DoSomething inp l h
                    output.Text <- data
                }
                |> Async.Start)
                    
        let str = TextArea [Attr.Style "width:670px; height:520px"]
        let inputFrame = 
            Div[
                lengthSpec
                ]-< [Attr.Style "width:100%"]
        
        Div [
            inputFrame
            HR []
            H4 [Attr.Class "text-muted"] -< [Text "The server responded:"]
            Div [Attr.Class "jumbotron"] -< [output]
        ]
