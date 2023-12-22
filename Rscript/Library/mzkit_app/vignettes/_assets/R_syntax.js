var Token;
(function (Token) {
    Token.html_color = /"[#][a-zA-Z0-9]{6}"/ig;
    /**
     * pattern for match the number token
    */
    Token.number_regexp = /[-]?\d+(\.\d+)?([eE][-]?\d+)?/ig;
    Token.symbol_name = /[a-zA-Z_\.]/ig;
    function renderTextSet(chars) {
        var set = {};
        for (var _i = 0, chars_1 = chars; _i < chars_1.length; _i++) {
            var char = chars_1[_i];
            set[char] = 1;
        }
        return set;
    }
    Token.logical = renderTextSet(["true", "false", "TRUE", "FALSE", "True", "False"]);
    Token.operators = renderTextSet(["+", "-", "*", "/", "\\", "!", "$", "%", "^", "&", "=", "<", ">", ":", "|", ",", "~", "?"]);
    Token.stacks = renderTextSet(["[", "]", "(", ")", "{", "}"]);
    Token.keywords = renderTextSet([
        "imports", "from", "require",
        "if", "else", "for", "break", "while",
        "function", "return",
        "let", "const",
        "stop", "invisible",
        "export", "namespace", "class",
        "string", "double", "integer", "list"
    ]);
})(Token || (Token = {}));
///<reference path="token.ts" />
var TokenParser = /** @class */ (function () {
    function TokenParser(source) {
        this.source = source;
        this.escaped = false;
        this.escape_char = null;
        this.escape_comment = false;
        /**
         * for get char at index
        */
        this.i = 0;
        this.str_len = -1;
        /**
         * the token text buffer
        */
        this.buf = [];
        if (source) {
            this.str_len = source.length;
        }
    }
    TokenParser.prototype.getTokens = function () {
        var tokens = [];
        var tmp = null;
        while (this.i < this.str_len) {
            if (tmp = this.walkChar(this.source.charAt(this.i++))) {
                tokens.push(tmp);
                if (this.buf.length == 1) {
                    var c = this.buf[0];
                    if (c == " " || c == "\t") {
                        this.buf = [];
                        tokens.push({
                            text: c, type: "whitespace"
                        });
                    }
                    else if (c == "\r" || c == "\n") {
                        this.buf = [];
                        tokens.push({
                            text: c, type: "newLine"
                        });
                    }
                    else if (c in Token.stacks) {
                        this.buf = [];
                        tokens.push({
                            text: c, type: "bracket"
                        });
                    }
                    else if (c == ";") {
                        this.buf = [];
                        tokens.push({
                            text: c, type: "terminator"
                        });
                    }
                    else if (c == ",") {
                        this.buf = [];
                        tokens.push({
                            text: c, type: "delimiter"
                        });
                    }
                }
            }
        }
        if (this.buf.length > 0) {
            tokens.push(this.measureToken(null));
        }
        return tokens;
    };
    TokenParser.prototype.walkChar = function (c) {
        if (this.escaped) {
            this.buf.push(c);
            if (c == this.escape_char) {
                var pull_str = this.buf.join("");
                var type = Token.html_color.test(pull_str) ? "color" : "character";
                // end escape
                this.escaped = false;
                this.escape_char = null;
                this.buf = [];
                return {
                    text: pull_str,
                    type: type
                };
            }
            else {
                // do nothing
            }
            return null;
        }
        if (this.escape_comment) {
            if (c == "\r" || c == "\n") {
                var pull_comment = this.buf.join("");
                // end comment line
                this.escape_comment = false;
                this.buf = [c];
                return {
                    text: pull_comment,
                    type: "comment"
                };
            }
            else {
                this.buf.push(c);
            }
            return null;
        }
        if (c == "#") {
            // start comment
            this.escape_comment = true;
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                this.buf.push(c);
            }
        }
        else if (c == "'" || c == '"') {
            // start string
            this.escape_char = c;
            this.escaped = true;
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                this.buf.push(c);
            }
        }
        else if (c == " " || c == "\t") {
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                return {
                    type: "whitespace",
                    text: c
                };
            }
        }
        else if (c == "\r" || c == "\n") {
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                return {
                    type: "newLine",
                    text: c
                };
            }
        }
        else if (c in Token.stacks) {
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                return {
                    type: "bracket",
                    text: c
                };
            }
        }
        else if (c == ";") {
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                return {
                    type: "terminator",
                    text: c
                };
            }
        }
        else if (c == ",") {
            if (this.buf.length > 0) {
                // populate previous token
                return this.measureToken(c);
            }
            else {
                return {
                    type: "delimiter",
                    text: c
                };
            }
        }
        else {
            this.buf.push(c);
        }
        return null;
    };
    TokenParser.prototype.measureToken = function (push_next) {
        var text = this.buf.join("");
        var test_symbol = text.match(Token.symbol_name);
        var test_number = text.match(Token.number_regexp);
        this.buf = [];
        if (push_next) {
            this.buf.push(push_next);
        }
        if (text == "NULL" || text == "NA" || text == "NaN" || text == "Inf") {
            return {
                text: text,
                type: "factor"
            };
        }
        else if (text in Token.logical) {
            return {
                text: text,
                type: "logical"
            };
        }
        else if (text in Token.keywords) {
            return {
                text: text,
                type: "keyword"
            };
        }
        else if (test_number && (test_number.length > 0)) {
            return {
                text: text,
                type: "number"
            };
        }
        else if (test_symbol && (test_symbol.length > 0)) {
            // symbol
            return {
                text: text,
                type: "symbol"
            };
        }
        else if (text in Token.operators) {
            return {
                text: text,
                type: "operator"
            };
        }
        else {
            return {
                text: text,
                type: "undefined"
            };
        }
    };
    return TokenParser;
}());
///<reference path="token.ts" />
///<reference path="parser.ts" />
function parseText(str) {
    var parser = new TokenParser(str);
    var tokens = parser.getTokens();
    return tokens;
}
/**
 * parse the script text to syntax highlight html content
*/
function highlights(str, verbose) {
    if (verbose === void 0) { verbose = true; }
    var html = "";
    var syntax = parseText(str);
    if (verbose) {
        console.log("view of the syntax tokens:");
        console.table(syntax);
    }
    for (var _i = 0, syntax_1 = syntax; _i < syntax_1.length; _i++) {
        var t = syntax_1[_i];
        switch (t.type) {
            case "newLine":
                html = html + "\n";
                break;
            case "whitespace":
            case "symbol":
                html = html + escape_op(t.text);
                break;
            case "color":
                html = html + ("<span class=\"color\" style=\"font-style: italic; background-color: " + t.text.replace(/'/ig, "").replace(/"/ig, "") + "; color: white; font-weight: bold;\">" + t.text + "</span>");
                break;
            default:
                html = html + ("<span class=\"" + t.type + "\">" + t.text + "</span>");
        }
    }
    return html;
}
function escape_op(str) {
    if (!str) {
        return "";
    }
    else {
        return str
            .replace("&", "&amp;")
            .replace(">", "&gt;")
            .replace("<", "&lt;");
    }
}
//# sourceMappingURL=R_syntax.js.map