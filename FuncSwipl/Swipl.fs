namespace funcswipl
open System
open System.Runtime.InteropServices
open NativeHeader

module Swipl =

    type Term = 
        | StringTerm of string
        | LongTerm of int64
        | Variable of string

    type TermRef = { Ptr:nativeint; Term:Term }

    type TermVRef = { Ptr:nativeint; Arity:int; Terms:Term array } 

    type CompoundTermRef = { Ptr:nativeint; Name:string; Terms:TermVRef }

    let newTerm t = 
        match t with
        | StringTerm s ->
            let pTerm = PL_new_term_ref()
            match PL_put_atom_chars(pTerm, s) with
            | 0 -> None
            | _ -> Some { Ptr=pTerm; Term=t }
        | LongTerm i ->
            let pTerm = PL_new_term_ref()
            match PL_put_integer(pTerm, i) with
            | 0 -> None
            | _ -> Some { Ptr=pTerm; Term=t }
        | Variable s ->
            let pTerm = PL_new_term_ref()
            PL_put_atom_chars(pTerm, s) |> ignore
            match PL_put_variable(pTerm) with
            | 0 -> None
            | _ -> Some { Ptr=pTerm; Term=t }

    let termVector (terms:Term array) =
        let pTermV = PL_new_term_refs(terms.Length)
        let results = 
            terms |> Array.mapi(fun i term -> 
                match term |> newTerm with
                | Some ref ->
                    match PL_put_term(pTermV + nativeint i, ref.Ptr) with
                    | 0 -> None
                    | _ -> Some ref
                | None -> None
            )
        match results |> Array.exists(fun ref -> match ref with | None -> true | _ -> false ) with
        | true -> 
            PL_reset_term_refs(pTermV)
            None
        | false -> Some { TermVRef.Ptr=pTermV; Arity=terms.Length; Terms=terms }

    let compoundTerm name termVector =
        match termVector with 
        | Some termV ->
            let pTerm = PL_new_term_ref()
            let pFunc = PL_new_functor(PL_new_atom(name), termV.Arity)
            match PL_cons_functor_v(pTerm, pFunc, termV.Ptr) with
            | 0 -> None
            | _ -> Some { CompoundTermRef.Ptr=pTerm; Name=name; Terms=termV }
        | None -> None

    let call name compoundTerm =
        match compoundTerm with 
        | Some term ->
            let pPred = PL_predicate(name, 1, "user")
            match PL_call_predicate(nativeint 0, PL_Q_NORMAL, pPred, term.Ptr) with 
            | 0 -> None
            | _ -> Some true
        | None -> None

    type Query(name, termVector) = 
        let pPred = PL_predicate(name, termVector.Arity, "user")
        let qid = PL_open_query(nativeint 0, PL_Q_CATCH_EXCEPTION, pPred, termVector.Ptr)
        let mutable disposed = false
        member x.MoveNext () =
            match disposed with
            | true -> raise (ObjectDisposedException("Cannot use disposed query."))
            | false -> 
                seq { while PL_next_solution(qid) <> 0 do yield termVector }
        interface IDisposable with
            member x.Dispose () = 
                match disposed with 
                | true -> ()
                | false -> PL_close_query(qid) |> ignore


    let (|IsIntTerm|_|) pTerm = 
        match pTerm |> PL_is_integer with
        | 0 -> None
        | _ ->
            let mutable i : int64 = 0L
            match PL_get_integer_ex(pTerm, &i) with
            | 0 -> None
            | _ -> Some {Ptr=pTerm; Term=LongTerm(i)}


    let (|IsStringTerm|_|) pTerm = 
        match pTerm |> PL_is_string with
        | 0 -> None
        | _ ->
            let mutable len = 0
            let mutable ptrStr = nativeint 0
            let flags = CVT_WRITE_CANONICAL ||| BUF_RING ||| REP_UTF8
            match PL_get_wchars(pTerm, &len, &ptrStr, flags) with
            | 0 -> None
            | _ ->
                let s = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptrStr, len*2)
                Some {Ptr=pTerm; Term=StringTerm(s)}


    let (|IsAtomTerm|_|) pTerm = 
        match pTerm |> PL_is_atom with
        | 0 -> None
        | _ ->
            let mutable len = 0
            let mutable ptrStr = nativeint 0
            let flags = CVT_WRITE_CANONICAL ||| BUF_RING ||| REP_UTF8
            match PL_get_wchars(pTerm, &len, &ptrStr, flags) with
            | 0 -> None
            | _ ->
                let s = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptrStr, len*2)
                Some { Ptr=pTerm; Term=StringTerm(s) }

    let parseTerm pTerm = 
        match pTerm with
        | IsIntTerm t -> t
        | IsStringTerm t -> t
        | IsAtomTerm t -> t
        | _ -> 
            let termType = PL_term_type(pTerm)
            failwith (sprintf "Unable to parse term of type %i." termType)

    type TermVRef with
        member x.GetTerm idx =
            match x.Arity > idx with
            | false -> raise(IndexOutOfRangeException())
            | true -> (x.Ptr + nativeint idx) |> parseTerm
