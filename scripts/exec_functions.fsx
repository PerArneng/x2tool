open System.Diagnostics;


let startProcess (procName:string) (args:array<string>):int =
    let procStartInfo = new ProcessStartInfo();
    procStartInfo.FileName <- procName
    procStartInfo.UseShellExecute <- false
    procStartInfo.Arguments <- (String.concat " " args)
    let proc = Process.Start(procStartInfo)
    proc.WaitForExit()
    proc.ExitCode

let processFailed (code:int):bool = code > 0

let code = startProcess "dotnet" [|""|]
printfn "process failed: %A" (processFailed code)