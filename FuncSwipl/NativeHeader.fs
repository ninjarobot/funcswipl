namespace funcswipl
open System.Runtime.InteropServices

module NativeHeader =

    [<Literal>] 
    let CVT_ATOM = 0x0001
    [<Literal>] 
    let CVT_STRING = 0x0002
    [<Literal>] 
    let CVT_LIST = 0x0004
    [<Literal>] 
    let CVT_INTEGER = 0x0008
    [<Literal>] 
    let CVT_FLOAT = 0x0010
    [<Literal>] 
    let CVT_VARIABLE = 0x0020
    [<Literal>] 
    let CVT_NUMBER = (CVT_INTEGER|||CVT_FLOAT)
    [<Literal>] 
    let CVT_ATOMIC = (CVT_NUMBER|||CVT_ATOM|||CVT_STRING)
    [<Literal>] 
    let CVT_WRITE = 0x0040
    [<Literal>] 
    let CVT_WRITE_CANONICAL = 0x0080
    [<Literal>] 
    let CVT_WRITEQ = 0x00C0
    [<Literal>] 
    let CVT_ALL = (CVT_ATOMIC|||CVT_LIST)
    [<Literal>] 
    let CVT_MASK = 0x00ff
    [<Literal>] 
    let REP_UTF8 = 0x1000
    [<Literal>] 
    let BUF_RING = 0x0100
    [<Literal>] 
    let PL_Q_NORMAL = 0x02
    [<Literal>]
    let PL_Q_NODEBUG = 0x04
    [<Literal>]
    let PL_Q_CATCH_EXCEPTION = 0x08
    [<Literal>]
    let PL_Q_PASS_EXCEPTION = 0x10


    [<DllImport("libswipl")>]
    extern int PL_initialise(int argc, string[] argv)
    [<DllImport("libswipl")>]
    extern int PL_halt(int status)
    [<DllImport("libswipl")>]
    extern int PL_cleanup(int status)
    [<DllImport("libswipl")>]
    extern int PL_thread_attach_engine(nativeint pThreadAttributes)
    [<DllImport("libswipl")>]
    extern int PL_thread_destroy_engine()
    [<DllImport("libswipl")>]
    extern nativeint PL_create_engine(nativeint attributes)
    [<DllImport("libswipl")>]
    extern int PL_destroy_engine(nativeint pEngine)
    [<DllImport("libswipl")>]
    extern nativeint PL_new_atom(string)
    [<DllImport("libswipl")>]
    extern nativeint PL_new_term_refs(int)
    [<DllImport("libswipl")>]
    extern nativeint PL_new_term_ref()
    [<DllImport("libswipl")>]
    extern nativeint PL_predicate(string name, int arity, string modul)
    [<DllImport("libswipl")>]
    extern int PL_put_atom_chars(nativeint pTerm, string chars)
    [<DllImport("libswipl")>]
    extern int PL_put_integer(nativeint pTerm, int64 i)
    [<DllImport("libswipl")>]
    extern int PL_get_integer_ex(nativeint pTerm, [<Out>] int64& pInt)
    [<DllImport("libswipl")>]
    extern int PL_put_variable(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_unify_int64(nativeint pTerm, int64 n)
    [<DllImport("libswipl")>]
    extern int PL_unify(nativeint pTerm1, nativeint pTerm2)
    [<DllImport("libswipl")>]
    extern int PL_wchars_to_term(string chars, nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_get_wchars(nativeint pTerm, [<Out>]int& len, [<Out>]nativeint& s, int flags)
    [<DllImport("libswipl")>]
    extern int PL_put_term(nativeint t1, nativeint t2)
    [<DllImport("libswipl")>]
    extern void PL_reset_term_refs(nativeint after)
    [<DllImport("libswipl")>]
    extern int PL_is_variable(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_is_atom(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_is_integer(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_is_string(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_is_float(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_is_compound(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern nativeint PL_new_functor(nativeint pAtom, int arity)
    [<DllImport("libswipl")>]
    extern int PL_cons_functor_v(nativeint pTerm, nativeint pFunctor, nativeint pTermV)
    [<DllImport("libswipl")>]
    extern int PL_term_type(nativeint pTerm)
    [<DllImport("libswipl")>]
    extern nativeint PL_open_query(nativeint pModule, int flags, nativeint pPredicate, nativeint pTerm)
    [<DllImport("libswipl")>]
    extern int PL_next_solution(nativeint pQuery)
    [<DllImport("libswipl")>]
    extern int PL_close_query(nativeint pQueryId)
    [<DllImport("libswipl")>]
    extern int PL_call(nativeint pTerm, nativeint pModule)
    [<DllImport("libswipl")>]
    extern int PL_call_predicate(nativeint pModule, int flags, nativeint pPredicate, nativeint pTerm)
    [<DllImport("libswipl")>]
    extern nativeint PL_open_foreign_frame()
    [<DllImport("libswipl")>]
    extern void PL_rewind_foreign_frame(nativeint frameId)
    [<DllImport("libswipl")>]
    extern void PL_discard_foreign_frame(nativeint frameId)
    [<DllImport("libswipl")>]
    extern void PL_close_foreign_frame(nativeint frameId)
    