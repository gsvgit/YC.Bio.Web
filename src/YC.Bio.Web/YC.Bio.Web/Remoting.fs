namespace YC.Bio.Web

open WebSharper

module Server =

    [<Rpc>]
    let DoSomething input l h =
        let R (s: string) = System.String(Array.rev(s.ToCharArray())).Substring(l, h)
        async {
            return R input
        }    
