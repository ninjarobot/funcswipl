namespace funcswipl
open System
open System.Runtime.InteropServices
open NativeHeader
open Swipl

module Program =

    [<EntryPoint>]
    let main argv =
        match Environment.GetEnvironmentVariable("SWI_HOME_DIR") with
        | null -> failwith "Set SWI_HOME_DIR environment variable before running."
        | _ -> ()

        match PL_initialise(2, [|System.Reflection.Assembly.GetExecutingAssembly().Location;"-q"|]) with
        | 0 -> PL_halt(1)
        | _ ->
            let pIncrementEngineCounter = PL_thread_attach_engine(nativeint 0)
            //let pEngine = PL_create_engine(nativeint 0)

            let termVRes = termVector([|StringTerm("testing"); LongTerm(100000L); StringTerm("another_test")|])
            printfn "Created term vector"

            match termVRes with
            | Some termV ->
                printfn "Term 1: %A" (termV.GetTerm(0))
                printfn "Term 2: %A" (termV.GetTerm(1))
                printfn "Term 3: %A" (termV.GetTerm(2))
                match (compoundTerm "cmpnd" termVRes) with
                | Some cTerm ->
                     printfn "Compound term created."
                | None -> printfn "Unable to create compound term."
            | None -> printfn "Couldn't create term vector."

            let fid = PL_open_foreign_frame()

            termVector([|StringTerm("child1"); LongTerm(5L)|])
            |> compoundTerm "age" 
            |> call "assertz" |> ignore
            call "assertz" (compoundTerm "age" (termVector([|StringTerm("child2"); LongTerm(3L)|]) ) ) |> ignore
            match termVector([|Variable("Child"); Variable("Age")|]) with
            | Some term2 ->
                use q = new Query("age", term2)
                q.MoveNext() |> Seq.iter(fun t ->
                    let resultTerm = (t.Ptr + nativeint 1) |> parseTerm
                    printfn "%A age %A" (t.GetTerm(0).Term) (t.GetTerm(1).Term)
                    )
            | _ -> ()

            termVector([|StringTerm("child1"); StringTerm("star wars")|]) |> compoundTerm "likes" |> call "assertz" |> ignore
            termVector([|StringTerm("child1"); StringTerm("robots")|]) |> compoundTerm "likes" |> call "assertz" |> ignore
            termVector([|StringTerm("child1"); StringTerm("pirates")|]) |> compoundTerm "likes" |> call "assertz" |> ignore
            termVector([|StringTerm("child2"); StringTerm("star wars")|]) |> compoundTerm "likes" |> call "assertz" |> ignore
            termVector([|StringTerm("child2"); StringTerm("frozen")|]) |> compoundTerm "likes" |> call "assertz" |> ignore
            termVector([|StringTerm("child2"); StringTerm("mermaids")|]) |> compoundTerm "likes" |> call "assertz" |> ignore



            [|Variable("_");Variable("_")|] |> termVector |> compoundTerm "likes" |> call "retractall" |> ignore

            PL_close_foreign_frame(fid) |> ignore

            match termVector([|StringTerm("child1"); Variable("Thing")|]) with
            | Some term2 ->
                use q = new Query("likes", term2)
                q.MoveNext() |> Seq.iter(fun t ->
                    let resultTerm = (t.Ptr + nativeint 1) |> parseTerm
                    printfn "child1 likes %A" (t.GetTerm(1).Term)
                )
            | _ -> ()
            match termVector([|StringTerm("child2"); Variable("Thing")|]) with
            | Some term2 ->
                use q = new Query("likes", term2)
                q.MoveNext() |> Seq.iter(fun t ->
                    let resultTerm = (t.Ptr + nativeint 1) |> parseTerm
                    printfn "child2 likes %A" (t.GetTerm(1).Term)
                )
            | _ -> ()
            match termVector([|Variable("Who"); StringTerm("star wars")|]) with
            | Some term2 ->
                use q = new Query("likes", term2)
                q.MoveNext() |> Seq.iter(fun t ->
                    let resultTerm = (t.Ptr + nativeint 1) |> parseTerm
                    printfn "%A likes Star Wars" (t.GetTerm(0).Term)
                )
            | _ -> ()

            termVector(Array.empty) |> compoundTerm "test.pl" |> call "consult" |> ignore

            match termVector([|StringTerm("great"); Variable("Disk")|]) with
            | Some term ->
                use q = new Query("disk_quality", term)
                q.MoveNext() |> Seq.iter(fun t ->
                    let resultTerm = (t.Ptr + nativeint 1) |> parseTerm
                    printfn "Great disk: %A" (t.GetTerm(1).Term)
                )
            | _ -> ()

            PL_thread_destroy_engine() |> ignore

            printfn "Engine detached from thread."

            //PL_destroy_engine(pEngine) |> ignore

            //printfn "Engine destroyed."

            let haltRes = PL_halt(0)

            printfn "Halted."

            haltRes
        |> ignore
        printfn "Exiting"
        0 // return an integer exit code
