
open System
open System.Diagnostics
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

type Bar =
    {
        value:int64;
        title:string
    }

type BarChart =
    {
        bars:array<Bar>;
        maxValue:int64;
        minValue:int64;
        titleWidth:int
        screenWidth:int;
    }


let createBarChart (bars:array<Bar>) (screenWidth:int):BarChart =
    let values = bars |> Array.map (fun bar -> bar.value)
    let minValue = values |> Array.min
    let maxValue = values |> Array.max
    let maxTitleWidth = int ((double screenWidth) * 0.5)
    let maxBarTitleLength = bars |> Array.map (fun bar -> bar.title.Length) |> Array.max
    let titleWidth = Math.Min(maxBarTitleLength, maxTitleWidth)
    
    {bars=bars; maxValue=maxValue; minValue=minValue; titleWidth=titleWidth; screenWidth=screenWidth}


let renderBar (barChart:BarChart) (bar:Bar):string =
    let charArrayLine = Array.create<char> barChart.screenWidth '.'
    
    let minTitleLength = Math.Min(barChart.titleWidth, bar.title.Length) 
    
    Array.Copy(bar.title.ToCharArray(), charArrayLine, minTitleLength)
    
    let barValueRangeSize = barChart.maxValue - barChart.minValue
    let valueNormalized = bar.value - barChart.minValue
    let percentageOfRange:double = (double valueNormalized) / (double barValueRangeSize)
    let totalNrOfBarChars = barChart.screenWidth - (barChart.titleWidth + 1)
    let valueBarCharsLength = int ((double totalNrOfBarChars) * percentageOfRange)
    
    let barChars = Array.create<char> valueBarCharsLength '#'
    
    Array.Copy(barChars, 0, charArrayLine, barChart.titleWidth + 1, barChars.Length)
     
    new String(charArrayLine)


let lineToBar (line:string):Result<Bar, string> =
    let r = new Regex("(\d+)(.*)")
    let matches = r.Match(line)
    if matches.Success then
        Ok {value=Int64.Parse(matches.Groups.[1].Value); 
            title=matches.Groups.[2].Value.Trim()}
    else
        Error (sprintf "The line did not match the specification: '%s'" line)


let stdinReader = new StreamReader(Console.OpenStandardInput())   

let mutable loop = true

let allLines (sr:StreamReader) = seq {
    while not sr.EndOfStream do
            yield sr.ReadLine ()
}

let bars = allLines stdinReader 
                |> Seq.map lineToBar
                |> Seq.filter (fun b -> match b with | Ok x -> true | _ -> false )
                |> Seq.map (fun b -> b)
                |> Seq.filter (fun b -> b.value > -1L)
                |> Seq.toArray
                |> Array.sortBy (fun b -> b.value)


stdinReader.Close()


let barChart = createBarChart bars 140

let barRendererForChart = (renderBar barChart)

barChart.bars
    |> Array.map barRendererForChart
    |> Array.iter (printfn "%s")
