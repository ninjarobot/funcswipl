namespace FuncSwiplTests
open System
open NUnit.Framework
open funcswipl.NativeHeader
open funcswipl.Swipl

[<TestFixture>]
type TermTests() = 

    let mutable pEngine = nativeint 0
    let mutable pFrame = nativeint 0

    [<TestFixtureSetUp>]
    member x.SetupFixture() =
        Environment.SetEnvironmentVariable("SWI_HOME_DIR", "/Applications/SWI-Prolog.app/Contents/swipl")
        match PL_initialise(2, [|System.Reflection.Assembly.GetExecutingAssembly().Location;"-q"|]) with
        | 0 -> PL_halt(1) |> ignore; failwith "Unable to initialize libswipl."
        | _ -> ()

    [<TestFixtureTearDown>]
    member x.TeardownFixutre() = 
        PL_halt(0) |> ignore

    [<SetUp>]
    member x.Setup() =
        let pIncrementEngineCounter = PL_thread_attach_engine(nativeint 0)
        pFrame <- PL_open_foreign_frame()
        //pEngine <- PL_create_engine(nativeint 0)
        ()

    [<TearDown>]
    member x.TearDown() =
        PL_discard_foreign_frame(pFrame) 
        PL_thread_destroy_engine() |> ignore
        //PL_destroy_engine(pEngine) |> ignore
        ()

    [<Test>]
    member x.CreateTerm() =
        let termRes = StringTerm("testing") |> newTerm
        match termRes with 
        | Some term -> Assert.AreNotEqual(nativeint 0, term.Ptr, "#1")
        | None -> Assert.Fail("#2")

    [<Test>]
    member x.CreateTermVector() =
        let termRes = termVector([|StringTerm("testing"); LongTerm(100000L); StringTerm("another_test")|])
        match termRes with 
        | Some term -> 
            Assert.AreNotEqual(nativeint 0, term.Ptr, "#1")
            Assert.AreEqual(3, term.Arity, "#2")
            match term.GetTerm(0).Term with
            | StringTerm s -> Assert.AreEqual("testing", s, "#3")
            | _ -> Assert.Fail("#4")
            match term.GetTerm(1).Term with
            | LongTerm l -> Assert.AreEqual(1000000L, l, "#5")
            | _ -> Assert.Fail("#6")
            match term.GetTerm(2).Term with
            | StringTerm s -> Assert.AreEqual("another_test", s, "#7")
            | _ -> Assert.Fail("#8")
        | None -> Assert.Fail("#3")

    [<Test>]
    member x.CreateCompoundTerm() =
        let compoundTerm = termVector([|StringTerm("child1"); LongTerm(5L)|]) |> compoundTerm "age"
        match compoundTerm with
        | Some term ->
            Assert.AreNotEqual(nativeint 0, term.Ptr, "#1")
        | None -> Assert.Fail("#2")

    [<Test>]
    member x.CallAssert() =
        let fid = PL_open_foreign_frame()
        termVector([|StringTerm("child1"); LongTerm(5L)|]) |> compoundTerm "age" |> call "assertz" |> ignore
        termVector([|StringTerm("child2"); LongTerm(3L)|]) |> compoundTerm "age" |> call "assertz" |> ignore
        PL_discard_foreign_frame(fid)

    [<Test>]
    member x.SolveCompound() =
        termVector([|StringTerm("child1"); LongTerm(5L)|]) |> compoundTerm "age" |> call "assertz" |> ignore
        termVector([|StringTerm("child2"); LongTerm(3L)|]) |> compoundTerm "age" |> call "assertz" |> ignore
        match termVector([|Variable("Child"); Variable("Age")|]) with
        | Some term ->
            use q = new Query("age", term)
            q.MoveNext() |> Seq.iter(fun t ->
                printfn "%A age %A" (t.GetTerm(0).Term) (t.GetTerm(1).Term)
                )
        | _ -> ()

        Assert.IsTrue(true)
