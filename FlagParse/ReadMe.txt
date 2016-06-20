Parsing is divided into three stages:

Tokenization. Input characters are scanned once left-to-right producing a sequence of four token types:
    String
    Flag
    Pole
    End

Escaping is handled in this stage.

Structuring. Tokens are visited sequentially to produce a sequence of two structure types:
    Output -- string tokens in isolation.
    Command -- A triple of string, structure-sequence, string.

Parsing. Structures are visited in a tree walk, producing a sequence of six instruction types:
    Output
    Render
    Loop
    LoopInline
    Call
    CallInline

Loop and Call will refer to names of other templates -- resolving these names into a sequence of instructions is external to this library.