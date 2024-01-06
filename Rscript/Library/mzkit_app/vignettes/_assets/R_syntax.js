var Token;
(function (Token) {
    /**
     * regexp for test html colors
    */
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
        "in", "between",
        "function", "return",
        "let", "const",
        "stop", "invisible",
        "export", "namespace", "class",
        "extends", "implements", // typescript keywords
        "string", "double", "integer", "list",
        "typeof"
    ]);
    var hex_colors = {
        aliceblue: "#F0F8FF",
        antiquewhite: "#FAEBD7",
        aqua: "#00FFFF",
        aquamarine: "#7FFFD4",
        azure: "#F0FFFF",
        beige: "#F5F5DC",
        bisque: "#FFE4C4",
        black: "#000000",
        blanchedalmond: "#FFEBCD",
        blue: "#0000FF",
        blueviolet: "#8A2BE2",
        brown: "#A52A2A",
        burlywood: "#DEB887",
        cadetblue: "#5F9EA0",
        chartreuse: "#7FFF00",
        chocolate: "#D2691E",
        coral: "#FF7F50",
        cornflowerblue: "#6495ED",
        cornsilk: "#FFF8DC",
        crimson: "#DC143C",
        cyan: "#00FFFF",
        darkblue: "#00008B",
        darkcyan: "#008B8B",
        darkgoldenrod: "#B8860B",
        darkgray: "#A9A9A9",
        darkgrey: "#A9A9A9",
        darkgreen: "#006400",
        darkkhaki: "#BDB76B",
        darkmagenta: "#8B008B",
        darkolivegreen: "#556B2F",
        darkorange: "#FF8C00",
        darkorchid: "#9932CC",
        darkred: "#8B0000",
        darksalmon: "#E9967A",
        darkseagreen: "#8FBC8F",
        darkslateblue: "#483D8B",
        darkslategray: "#2F4F4F",
        darkslategrey: "#2F4F4F",
        darkturquoise: "#00CED1",
        darkviolet: "#9400D3",
        deeppink: "#FF1493",
        deepskyblue: "#00BFFF",
        dimgray: "#696969",
        dimgrey: "#696969",
        dodgerblue: "#1E90FF",
        firebrick: "#B22222",
        floralwhite: "#FFFAF0",
        forestgreen: "#228B22",
        fuchsia: "#FF00FF",
        gainsboro: "#DCDCDC",
        ghostwhite: "#F8F8FF",
        gold: "#FFD700",
        goldenrod: "#DAA520",
        gray: "#808080",
        grey: "#808080",
        green: "#008000",
        greenyellow: "#ADFF2F",
        honeydew: "#F0FFF0",
        hotpink: "#FF69B4",
        indianred: "#CD5C5C",
        indigo: "#4B0082",
        ivory: "#FFFFF0",
        khaki: "#F0E68C",
        lavender: "#E6E6FA",
        lavenderblush: "#FFF0F5",
        lawngreen: "#7CFC00",
        lemonchiffon: "#FFFACD",
        lightblue: "#ADD8E6",
        lightcoral: "#F08080",
        lightcyan: "#E0FFFF",
        lightgoldenrodyellow: "#FAFAD2",
        lightgray: "#D3D3D3",
        lightgrey: "#D3D3D3",
        lightgreen: "#90EE90",
        lightpink: "#FFB6C1",
        lightsalmon: "#FFA07A",
        lightseagreen: "#20B2AA",
        lightskyblue: "#87CEFA",
        lightslategray: "#778899",
        lightslategrey: "#778899",
        lightsteelblue: "#B0C4DE",
        lightyellow: "#FFFFE0",
        lime: "#00FF00",
        limegreen: "#32CD32",
        linen: "#FAF0E6",
        magenta: "#FF00FF",
        maroon: "#800000",
        mediumaquamarine: "#66CDAA",
        mediumblue: "#0000CD",
        mediumorchid: "#BA55D3",
        mediumpurple: "#9370DB",
        mediumseagreen: "#3CB371",
        mediumslateblue: "#7B68EE",
        mediumspringgreen: "#00FA9A",
        mediumturquoise: "#48D1CC",
        mediumvioletred: "#C71585",
        midnightblue: "#191970",
        mintcream: "#F5FFFA",
        mistyrose: "#FFE4E1",
        moccasin: "#FFE4B5",
        navajowhite: "#FFDEAD",
        navy: "#000080",
        oldlace: "#FDF5E6",
        olive: "#808000",
        olivedrab: "#6B8E23",
        orange: "#FFA500",
        orangered: "#FF4500",
        orchid: "#DA70D6",
        palegoldenrod: "#EEE8AA",
        palegreen: "#98FB98",
        paleturquoise: "#AFEEEE",
        palevioletred: "#DB7093",
        papayawhip: "#FFEFD5",
        peachpuff: "#FFDAB9",
        peru: "#CD853F",
        pink: "#FFC0CB",
        plum: "#DDA0DD",
        powderblue: "#B0E0E6",
        purple: "#800080",
        rebeccapurple: "#663399",
        red: "#FF0000",
        rosybrown: "#BC8F8F",
        royalblue: "#4169E1",
        saddlebrown: "#8B4513",
        salmon: "#FA8072",
        sandybrown: "#F4A460",
        seagreen: "#2E8B57",
        seashell: "#FFF5EE",
        sienna: "#A0522D",
        silver: "#C0C0C0",
        skyblue: "#87CEEB",
        slateblue: "#6A5ACD",
        slategray: "#708090",
        slategrey: "#708090",
        snow: "#FFFAFA",
        springgreen: "#00FF7F",
        steelblue: "#4682B4",
        tan: "#D2B48C",
        teal: "#008080",
        thistle: "#D8BFD8",
        tomato: "#FF6347",
        turquoise: "#40E0D0",
        violet: "#EE82EE",
        wheat: "#F5DEB3",
        white: "#FFFFFF",
        whitesmoke: "#F5F5F5",
        yellow: "#FFFF00",
        yellowgreen: "#9ACD32",
    };
    function isColorLiteral(pull_str) {
        if (!Token.html_color) {
            return false;
        }
        if (Token.html_color.test(pull_str)) {
            return true;
        }
        else {
            pull_str = pull_str.toLowerCase();
            return pull_str in hex_colors;
        }
    }
    Token.isColorLiteral = isColorLiteral;
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
                var type = Token.isColorLiteral(pull_str) ? "color" : "character";
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
                html = html + "<span class=\"color\" style=\"font-style: italic; background-color: ".concat(t.text.replace(/'/ig, "").replace(/"/ig, ""), "; color: white; font-weight: bold;\">").concat(t.text, "</span>");
                break;
            default:
                html = html + "<span class=\"".concat(t.type, "\">").concat(t.text, "</span>");
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