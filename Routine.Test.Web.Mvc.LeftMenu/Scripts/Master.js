/*
* jQuery Form Plugin; v20140218
* http://jquery.malsup.com/form/
* Copyright (c) 2014 M. Alsup; Dual licensed: MIT/GPL
* https://github.com/malsup/form#copyright-and-license
*/
; !function (a) { "use strict"; "function" == typeof define && define.amd ? define(["jquery"], a) : a("undefined" != typeof jQuery ? jQuery : window.Zepto) }(function (a) { "use strict"; function b(b) { var c = b.data; b.isDefaultPrevented() || (b.preventDefault(), a(b.target).ajaxSubmit(c)) } function c(b) { var c = b.target, d = a(c); if (!d.is("[type=submit],[type=image]")) { var e = d.closest("[type=submit]"); if (0 === e.length) return; c = e[0] } var f = this; if (f.clk = c, "image" == c.type) if (void 0 !== b.offsetX) f.clk_x = b.offsetX, f.clk_y = b.offsetY; else if ("function" == typeof a.fn.offset) { var g = d.offset(); f.clk_x = b.pageX - g.left, f.clk_y = b.pageY - g.top } else f.clk_x = b.pageX - c.offsetLeft, f.clk_y = b.pageY - c.offsetTop; setTimeout(function () { f.clk = f.clk_x = f.clk_y = null }, 100) } function d() { if (a.fn.ajaxSubmit.debug) { var b = "[jquery.form] " + Array.prototype.join.call(arguments, ""); window.console && window.console.log ? window.console.log(b) : window.opera && window.opera.postError && window.opera.postError(b) } } var e = {}; e.fileapi = void 0 !== a("<input type='file'/>").get(0).files, e.formdata = void 0 !== window.FormData; var f = !!a.fn.prop; a.fn.attr2 = function () { if (!f) return this.attr.apply(this, arguments); var a = this.prop.apply(this, arguments); return a && a.jquery || "string" == typeof a ? a : this.attr.apply(this, arguments) }, a.fn.ajaxSubmit = function (b) { function c(c) { var d, e, f = a.param(c, b.traditional).split("&"), g = f.length, h = []; for (d = 0; g > d; d++) f[d] = f[d].replace(/\+/g, " "), e = f[d].split("="), h.push([decodeURIComponent(e[0]), decodeURIComponent(e[1])]); return h } function g(d) { for (var e = new FormData, f = 0; f < d.length; f++) e.append(d[f].name, d[f].value); if (b.extraData) { var g = c(b.extraData); for (f = 0; f < g.length; f++) g[f] && e.append(g[f][0], g[f][1]) } b.data = null; var h = a.extend(!0, {}, a.ajaxSettings, b, { contentType: !1, processData: !1, cache: !1, type: i || "POST" }); b.uploadProgress && (h.xhr = function () { var c = a.ajaxSettings.xhr(); return c.upload && c.upload.addEventListener("progress", function (a) { var c = 0, d = a.loaded || a.position, e = a.total; a.lengthComputable && (c = Math.ceil(d / e * 100)), b.uploadProgress(a, d, e, c) }, !1), c }), h.data = null; var j = h.beforeSend; return h.beforeSend = function (a, c) { c.data = b.formData ? b.formData : e, j && j.call(this, a, c) }, a.ajax(h) } function h(c) { function e(a) { var b = null; try { a.contentWindow && (b = a.contentWindow.document) } catch (c) { d("cannot get iframe.contentWindow document: " + c) } if (b) return b; try { b = a.contentDocument ? a.contentDocument : a.document } catch (c) { d("cannot get iframe.contentDocument: " + c), b = a.document } return b } function g() { function b() { try { var a = e(r).readyState; d("state = " + a), a && "uninitialized" == a.toLowerCase() && setTimeout(b, 50) } catch (c) { d("Server abort: ", c, " (", c.name, ")"), h(A), w && clearTimeout(w), w = void 0 } } var c = l.attr2("target"), f = l.attr2("action"), g = "multipart/form-data", j = l.attr("enctype") || l.attr("encoding") || g; x.setAttribute("target", o), (!i || /post/i.test(i)) && x.setAttribute("method", "POST"), f != m.url && x.setAttribute("action", m.url), m.skipEncodingOverride || i && !/post/i.test(i) || l.attr({ encoding: "multipart/form-data", enctype: "multipart/form-data" }), m.timeout && (w = setTimeout(function () { v = !0, h(z) }, m.timeout)); var k = []; try { if (m.extraData) for (var n in m.extraData) m.extraData.hasOwnProperty(n) && (a.isPlainObject(m.extraData[n]) && m.extraData[n].hasOwnProperty("name") && m.extraData[n].hasOwnProperty("value") ? k.push(a('<input type="hidden" name="' + m.extraData[n].name + '">').val(m.extraData[n].value).appendTo(x)[0]) : k.push(a('<input type="hidden" name="' + n + '">').val(m.extraData[n]).appendTo(x)[0])); m.iframeTarget || q.appendTo("body"), r.attachEvent ? r.attachEvent("onload", h) : r.addEventListener("load", h, !1), setTimeout(b, 15); try { x.submit() } catch (p) { var s = document.createElement("form").submit; s.apply(x) } } finally { x.setAttribute("action", f), x.setAttribute("enctype", j), c ? x.setAttribute("target", c) : l.removeAttr("target"), a(k).remove() } } function h(b) { if (!s.aborted && !F) { if (E = e(r), E || (d("cannot access response document"), b = A), b === z && s) return s.abort("timeout"), y.reject(s, "timeout"), void 0; if (b == A && s) return s.abort("server abort"), y.reject(s, "error", "server abort"), void 0; if (E && E.location.href != m.iframeSrc || v) { r.detachEvent ? r.detachEvent("onload", h) : r.removeEventListener("load", h, !1); var c, f = "success"; try { if (v) throw "timeout"; var g = "xml" == m.dataType || E.XMLDocument || a.isXMLDoc(E); if (d("isXml=" + g), !g && window.opera && (null === E.body || !E.body.innerHTML) && --G) return d("requeing onLoad callback, DOM not available"), setTimeout(h, 250), void 0; var i = E.body ? E.body : E.documentElement; s.responseText = i ? i.innerHTML : null, s.responseXML = E.XMLDocument ? E.XMLDocument : E, g && (m.dataType = "xml"), s.getResponseHeader = function (a) { var b = { "content-type": m.dataType }; return b[a.toLowerCase()] }, i && (s.status = Number(i.getAttribute("status")) || s.status, s.statusText = i.getAttribute("statusText") || s.statusText); var j = (m.dataType || "").toLowerCase(), k = /(json|script|text)/.test(j); if (k || m.textarea) { var l = E.getElementsByTagName("textarea")[0]; if (l) s.responseText = l.value, s.status = Number(l.getAttribute("status")) || s.status, s.statusText = l.getAttribute("statusText") || s.statusText; else if (k) { var o = E.getElementsByTagName("pre")[0], p = E.getElementsByTagName("body")[0]; o ? s.responseText = o.textContent ? o.textContent : o.innerText : p && (s.responseText = p.textContent ? p.textContent : p.innerText) } } else "xml" == j && !s.responseXML && s.responseText && (s.responseXML = H(s.responseText)); try { D = J(s, j, m) } catch (t) { f = "parsererror", s.error = c = t || f } } catch (t) { d("error caught: ", t), f = "error", s.error = c = t || f } s.aborted && (d("upload aborted"), f = null), s.status && (f = s.status >= 200 && s.status < 300 || 304 === s.status ? "success" : "error"), "success" === f ? (m.success && m.success.call(m.context, D, "success", s), y.resolve(s.responseText, "success", s), n && a.event.trigger("ajaxSuccess", [s, m])) : f && (void 0 === c && (c = s.statusText), m.error && m.error.call(m.context, s, f, c), y.reject(s, "error", c), n && a.event.trigger("ajaxError", [s, m, c])), n && a.event.trigger("ajaxComplete", [s, m]), n && !--a.active && a.event.trigger("ajaxStop"), m.complete && m.complete.call(m.context, s, f), F = !0, m.timeout && clearTimeout(w), setTimeout(function () { m.iframeTarget ? q.attr("src", m.iframeSrc) : q.remove(), s.responseXML = null }, 100) } } } var j, k, m, n, o, q, r, s, t, u, v, w, x = l[0], y = a.Deferred(); if (y.abort = function (a) { s.abort(a) }, c) for (k = 0; k < p.length; k++) j = a(p[k]), f ? j.prop("disabled", !1) : j.removeAttr("disabled"); if (m = a.extend(!0, {}, a.ajaxSettings, b), m.context = m.context || m, o = "jqFormIO" + (new Date).getTime(), m.iframeTarget ? (q = a(m.iframeTarget), u = q.attr2("name"), u ? o = u : q.attr2("name", o)) : (q = a('<iframe name="' + o + '" src="' + m.iframeSrc + '" />'), q.css({ position: "absolute", top: "-1000px", left: "-1000px" })), r = q[0], s = { aborted: 0, responseText: null, responseXML: null, status: 0, statusText: "n/a", getAllResponseHeaders: function () { }, getResponseHeader: function () { }, setRequestHeader: function () { }, abort: function (b) { var c = "timeout" === b ? "timeout" : "aborted"; d("aborting upload... " + c), this.aborted = 1; try { r.contentWindow.document.execCommand && r.contentWindow.document.execCommand("Stop") } catch (e) { } q.attr("src", m.iframeSrc), s.error = c, m.error && m.error.call(m.context, s, c, b), n && a.event.trigger("ajaxError", [s, m, c]), m.complete && m.complete.call(m.context, s, c) } }, n = m.global, n && 0 === a.active++ && a.event.trigger("ajaxStart"), n && a.event.trigger("ajaxSend", [s, m]), m.beforeSend && m.beforeSend.call(m.context, s, m) === !1) return m.global && a.active--, y.reject(), y; if (s.aborted) return y.reject(), y; t = x.clk, t && (u = t.name, u && !t.disabled && (m.extraData = m.extraData || {}, m.extraData[u] = t.value, "image" == t.type && (m.extraData[u + ".x"] = x.clk_x, m.extraData[u + ".y"] = x.clk_y))); var z = 1, A = 2, B = a("meta[name=csrf-token]").attr("content"), C = a("meta[name=csrf-param]").attr("content"); C && B && (m.extraData = m.extraData || {}, m.extraData[C] = B), m.forceSync ? g() : setTimeout(g, 10); var D, E, F, G = 50, H = a.parseXML || function (a, b) { return window.ActiveXObject ? (b = new ActiveXObject("Microsoft.XMLDOM"), b.async = "false", b.loadXML(a)) : b = (new DOMParser).parseFromString(a, "text/xml"), b && b.documentElement && "parsererror" != b.documentElement.nodeName ? b : null }, I = a.parseJSON || function (a) { return window.eval("(" + a + ")") }, J = function (b, c, d) { var e = b.getResponseHeader("content-type") || "", f = "xml" === c || !c && e.indexOf("xml") >= 0, g = f ? b.responseXML : b.responseText; return f && "parsererror" === g.documentElement.nodeName && a.error && a.error("parsererror"), d && d.dataFilter && (g = d.dataFilter(g, c)), "string" == typeof g && ("json" === c || !c && e.indexOf("json") >= 0 ? g = I(g) : ("script" === c || !c && e.indexOf("javascript") >= 0) && a.globalEval(g)), g }; return y } if (!this.length) return d("ajaxSubmit: skipping submit process - no element selected"), this; var i, j, k, l = this; "function" == typeof b ? b = { success: b } : void 0 === b && (b = {}), i = b.type || this.attr2("method"), j = b.url || this.attr2("action"), k = "string" == typeof j ? a.trim(j) : "", k = k || window.location.href || "", k && (k = (k.match(/^([^#]+)/) || [])[1]), b = a.extend(!0, { url: k, success: a.ajaxSettings.success, type: i || a.ajaxSettings.type, iframeSrc: /^https/i.test(window.location.href || "") ? "javascript:false" : "about:blank" }, b); var m = {}; if (this.trigger("form-pre-serialize", [this, b, m]), m.veto) return d("ajaxSubmit: submit vetoed via form-pre-serialize trigger"), this; if (b.beforeSerialize && b.beforeSerialize(this, b) === !1) return d("ajaxSubmit: submit aborted via beforeSerialize callback"), this; var n = b.traditional; void 0 === n && (n = a.ajaxSettings.traditional); var o, p = [], q = this.formToArray(b.semantic, p); if (b.data && (b.extraData = b.data, o = a.param(b.data, n)), b.beforeSubmit && b.beforeSubmit(q, this, b) === !1) return d("ajaxSubmit: submit aborted via beforeSubmit callback"), this; if (this.trigger("form-submit-validate", [q, this, b, m]), m.veto) return d("ajaxSubmit: submit vetoed via form-submit-validate trigger"), this; var r = a.param(q, n); o && (r = r ? r + "&" + o : o), "GET" == b.type.toUpperCase() ? (b.url += (b.url.indexOf("?") >= 0 ? "&" : "?") + r, b.data = null) : b.data = r; var s = []; if (b.resetForm && s.push(function () { l.resetForm() }), b.clearForm && s.push(function () { l.clearForm(b.includeHidden) }), !b.dataType && b.target) { var t = b.success || function () { }; s.push(function (c) { var d = b.replaceTarget ? "replaceWith" : "html"; a(b.target)[d](c).each(t, arguments) }) } else b.success && s.push(b.success); if (b.success = function (a, c, d) { for (var e = b.context || this, f = 0, g = s.length; g > f; f++) s[f].apply(e, [a, c, d || l, l]) }, b.error) { var u = b.error; b.error = function (a, c, d) { var e = b.context || this; u.apply(e, [a, c, d, l]) } } if (b.complete) { var v = b.complete; b.complete = function (a, c) { var d = b.context || this; v.apply(d, [a, c, l]) } } var w = a("input[type=file]:enabled", this).filter(function () { return "" !== a(this).val() }), x = w.length > 0, y = "multipart/form-data", z = l.attr("enctype") == y || l.attr("encoding") == y, A = e.fileapi && e.formdata; d("fileAPI :" + A); var B, C = (x || z) && !A; b.iframe !== !1 && (b.iframe || C) ? b.closeKeepAlive ? a.get(b.closeKeepAlive, function () { B = h(q) }) : B = h(q) : B = (x || z) && A ? g(q) : a.ajax(b), l.removeData("jqxhr").data("jqxhr", B); for (var D = 0; D < p.length; D++) p[D] = null; return this.trigger("form-submit-notify", [this, b]), this }, a.fn.ajaxForm = function (e) { if (e = e || {}, e.delegation = e.delegation && a.isFunction(a.fn.on), !e.delegation && 0 === this.length) { var f = { s: this.selector, c: this.context }; return !a.isReady && f.s ? (d("DOM not ready, queuing ajaxForm"), a(function () { a(f.s, f.c).ajaxForm(e) }), this) : (d("terminating; zero elements found by selector" + (a.isReady ? "" : " (DOM not ready)")), this) } return e.delegation ? (a(document).off("submit.form-plugin", this.selector, b).off("click.form-plugin", this.selector, c).on("submit.form-plugin", this.selector, e, b).on("click.form-plugin", this.selector, e, c), this) : this.ajaxFormUnbind().bind("submit.form-plugin", e, b).bind("click.form-plugin", e, c) }, a.fn.ajaxFormUnbind = function () { return this.unbind("submit.form-plugin click.form-plugin") }, a.fn.formToArray = function (b, c) { var d = []; if (0 === this.length) return d; var f, g = this[0], h = this.attr("id"), i = b ? g.getElementsByTagName("*") : g.elements; if (i && !/MSIE 8/.test(navigator.userAgent) && (i = a(i).get()), h && (f = a(":input[form=" + h + "]").get(), f.length && (i = (i || []).concat(f))), !i || !i.length) return d; var j, k, l, m, n, o, p; for (j = 0, o = i.length; o > j; j++) if (n = i[j], l = n.name, l && !n.disabled) if (b && g.clk && "image" == n.type) g.clk == n && (d.push({ name: l, value: a(n).val(), type: n.type }), d.push({ name: l + ".x", value: g.clk_x }, { name: l + ".y", value: g.clk_y })); else if (m = a.fieldValue(n, !0), m && m.constructor == Array) for (c && c.push(n), k = 0, p = m.length; p > k; k++) d.push({ name: l, value: m[k] }); else if (e.fileapi && "file" == n.type) { c && c.push(n); var q = n.files; if (q.length) for (k = 0; k < q.length; k++) d.push({ name: l, value: q[k], type: n.type }); else d.push({ name: l, value: "", type: n.type }) } else null !== m && "undefined" != typeof m && (c && c.push(n), d.push({ name: l, value: m, type: n.type, required: n.required })); if (!b && g.clk) { var r = a(g.clk), s = r[0]; l = s.name, l && !s.disabled && "image" == s.type && (d.push({ name: l, value: r.val() }), d.push({ name: l + ".x", value: g.clk_x }, { name: l + ".y", value: g.clk_y })) } return d }, a.fn.formSerialize = function (b) { return a.param(this.formToArray(b)) }, a.fn.fieldSerialize = function (b) { var c = []; return this.each(function () { var d = this.name; if (d) { var e = a.fieldValue(this, b); if (e && e.constructor == Array) for (var f = 0, g = e.length; g > f; f++) c.push({ name: d, value: e[f] }); else null !== e && "undefined" != typeof e && c.push({ name: this.name, value: e }) } }), a.param(c) }, a.fn.fieldValue = function (b) { for (var c = [], d = 0, e = this.length; e > d; d++) { var f = this[d], g = a.fieldValue(f, b); null === g || "undefined" == typeof g || g.constructor == Array && !g.length || (g.constructor == Array ? a.merge(c, g) : c.push(g)) } return c }, a.fieldValue = function (b, c) { var d = b.name, e = b.type, f = b.tagName.toLowerCase(); if (void 0 === c && (c = !0), c && (!d || b.disabled || "reset" == e || "button" == e || ("checkbox" == e || "radio" == e) && !b.checked || ("submit" == e || "image" == e) && b.form && b.form.clk != b || "select" == f && -1 == b.selectedIndex)) return null; if ("select" == f) { var g = b.selectedIndex; if (0 > g) return null; for (var h = [], i = b.options, j = "select-one" == e, k = j ? g + 1 : i.length, l = j ? g : 0; k > l; l++) { var m = i[l]; if (m.selected) { var n = m.value; if (n || (n = m.attributes && m.attributes.value && !m.attributes.value.specified ? m.text : m.value), j) return n; h.push(n) } } return h } return a(b).val() }, a.fn.clearForm = function (b) { return this.each(function () { a("input,select,textarea", this).clearFields(b) }) }, a.fn.clearFields = a.fn.clearInputs = function (b) { var c = /^(?:color|date|datetime|email|month|number|password|range|search|tel|text|time|url|week)$/i; return this.each(function () { var d = this.type, e = this.tagName.toLowerCase(); c.test(d) || "textarea" == e ? this.value = "" : "checkbox" == d || "radio" == d ? this.checked = !1 : "select" == e ? this.selectedIndex = -1 : "file" == d ? /MSIE/.test(navigator.userAgent) ? a(this).replaceWith(a(this).clone(!0)) : a(this).val("") : b && (b === !0 && /hidden/.test(d) || "string" == typeof b && a(this).is(b)) && (this.value = "") }) }, a.fn.resetForm = function () { return this.each(function () { ("function" == typeof this.reset || "object" == typeof this.reset && !this.reset.nodeType) && this.reset() }) }, a.fn.enable = function (a) { return void 0 === a && (a = !0), this.each(function () { this.disabled = !a }) }, a.fn.selected = function (b) { return void 0 === b && (b = !0), this.each(function () { var c = this.type; if ("checkbox" == c || "radio" == c) this.checked = b; else if ("option" == this.tagName.toLowerCase()) { var d = a(this).parent("select"); b && d[0] && "select-one" == d[0].type && d.find("option").selected(!1), this.selected = b } }) }, a.fn.ajaxSubmit.debug = !1 });

$(function () {
	$('dd.single-value').each(function () {
		$(this).css("width", (window.innerWidth - 602));
	});
	$("dl.data-list").each(function () {
		$(this).css("width", (window.innerWidth - 302));
	});
	$(".operation-tabs dl.parameter-list dd").each(function () {
		$(this).css("width", (window.innerWidth - 322));
	});
	$("legend").each(function () {
		$(this).css("width", (window.innerWidth - 302));
	});
	$(".menu .parent").each(function () {
		$(this).attr("clicked", "0");
	});
	$(".menu .parent").mousedown(function () {
		$(this).children("a").css("border", "2px solid lightgray");
		$(this).children("a").css("height", "26px");
	});
	$(".menu ").css("height", window.innerHeight);
	$(".page-content ").css("height", (window.innerHeight - 20));
	$(window).resize(function () {
		$(".menu ").css("height", window.innerHeight);
		$(".page-content ").css("height", (window.innerHeight - 20));
	});
	$(".menu .parent").mouseup(function () {
		$(this).children("a").css("border", "none");
		$(this).children("a").css("height", "100%");
	});
	$(".menu .parent").click(function () {
		var prnt = $(this);
		prnt.children("a").css("pointer-events", "auto");
		var c = parseInt(prnt.attr("clicked")) + 1;
		prnt.attr("clicked", c);
		setTimeout(function () {
			var x = prnt.attr('class').split(/\s+/)[1];
			$(".menu .children").each(function () {
				if ($(".menu ." + x).attr("clicked") == 1) {
					var child = $(this);
					if (child.hasClass(x)) {
						if (child.children("a").length > 0) {
							if (parseInt(child.css("height")) <= 0) {
								show(child, function (p) { return p; });
							}
						}
					} else {
						if (parseInt(child.css("height")) > 0) {
							hide(child, function (p) { return p; });
						}
					}
				}
			});
			prnt.children("a").css("pointer-events", "none");
			prnt.attr("clicked", "0");
		}, 200);

	});
	render($(document));
	
});


function animate(opts) {
	var start = new Date;

	var id = setInterval(function () {
		var timePassed = new Date - start;
		var progress = timePassed / opts.duration;

		if (progress > 1) progress = 1;

		var delta = opts.delta(progress);
		opts.step(delta);

		if (progress == 1) {
			clearInterval(id);
		}
	}, opts.delay || 10);

}
function show(element, delta, duration) {
	var to = element.children("a").length * 30;

	animate({
		delay: 10,
		duration: duration || 300, // 1 sec by default
		delta: delta,
		step: function (delta) {
			element.css('height', to * delta + "px");
			var counter = 1;
			$(element).children("a").each(function () {
				$(this).css("top", -30 + (30 * delta * counter));
				counter++;
			});
			counter = 1;
			$(element).children("img").each(function () {
				$(this).css("top", -30 + (30 * delta * counter));
				counter++;
			});
		}
	});
}
function hide(element, delta, duration) {
	var to = element.children("a").length * 30;

	animate({
		delay: 10,
		duration: duration || 300, // 1 sec by default
		delta: delta,
		step: function (delta) {
			element.css('height', (to - (to * delta)) + "px");
			var counter = 0;
			$(element).children("a").each(function () {
				$(this).css("top", (30 * counter) - (30 * (counter + 1) * delta));
				counter++;
			});
			counter = 1;
			$(element).children("img").each(function () {
				$(this).css("top", (30 * counter) - (30 * (counter + 1) * delta));
				counter++;
			});
		}
	});
}


function render(obj) {
	$(obj).find("input[type=button]").each(function () { $(this).jqxButton(); });
	$(obj).find("input[type=submit]").each(function () { $(this).jqxButton(); });
	$(obj).find(".operation-buttons input[type=button].operation-button").each(function () {
		$(this).jqxButton({ theme: "metro" });
		$(this).addClass("operation-button");
	});
	$(obj).find(".operation-buttons input[type=submit].operation-button").each(function () {
		$(this).jqxButton({ theme: "metro" });
		$(this).addClass("operation-button");
	});
	$(obj).find("form.operation-form input[type=button]").click(function () {
		if ($(this).attr('value') == "Cancel") {
			$(this).closest("form")[0].reset();
			return;
		}

		if ($(this).attr('data-type') == "modal") {
			if (!($(this).prev().attr('data-return') == undefined)) {
				$('#' + $(this).prev().attr('data-return')).jqxWindow('open');
				return;
			}

			$(this).prev().attr('data-return', 'waiting');
			$.get($(this).attr('data-route'), function (data) {
				var modal = $("<div><div class='header'>Search</div><div class='modal-content'>" + data + "</div></div>")
					.jqxWindow({
						maxHeight: (window.innerHeight) + 'px',
						maxWidth: (window.innerWidth) + 'px',
						height: (window.innerHeight - 100) + 'px',
						width: (window.innerWidth - 250) + 'px',
						isModal: true,
						resizable: false
					});
				
				$('input[data-return=waiting]').attr('data-return', modal.attr('id'));

				render(modal);
			});
			return;
		}
	});
	$(obj).find(".operation-tabs form.operation-form input[type=button]").click(function () {
		if ($(this).attr('value') == "Cancel") {
			$(".operation-tabs").jqxTabs('collapse');
		} 
	});
	$(obj).find("input[type=button].operation-button").click(function () {
		var modalDiv = $('#' + $(this).attr("data-modal-id"));
		if (!(modalDiv.attr('data-modal-applied') == undefined)) {
			modalDiv.jqxWindow('open');
			return;
		}

		modalDiv.attr("data-modal-applied", "true");
		modalDiv.show();
		modalDiv.jqxWindow({ isModal: true, width: '382px', resizable: false });
	});
	$(obj).find(".operation-form-div form.operation-form input[type=button]").click(function () {
		if ($(this).attr('value') == "Cancel") {
			$('#' + $(this).attr('data-modal-id')).jqxWindow('close');
		}
	});
	$(obj).find("div.messages").click(function () {
		$(this).remove();
	});

	$(obj).find("table").each(function () {
		var columns = [];
		var count = $(this).children(":first").children(":first").children(".data-column").length;
		if (count < 3) {
			$(this).children(":first").children(":first").children(".data-column").each(function () {
				columns.push({ text: $(this).html(), dataField: $(this).text(), width: ((window.innerWidth - 302) / count) });
			});
		} else {
			$(this).children(":first").children(":first").children(".data-column").each(function () {
				columns.push({ text: $(this).html(), dataField: $(this).text() });
			});
		}
		$(this).jqxDataTable({ width: (window.innerWidth - 302), sortable: true, columnsResize: true, filterable: $(this).children("tbody").children("tr").length > 20, columns: columns, filterMode: "simple" });

		$('#' + $(this).attr('id')).on('rowClick', function (event) {
			if (event.args.originalEvent.which != 3) {
				return;
			}

			$(this)
				.next(".context-menus:first")
				.children(".context-menu-" + event.args.index)
				.delay(100)
				.css('left', event.args.originalEvent.pageX - 5)
				.css('top', event.args.originalEvent.pageY - 5)
				.fadeIn(50);
		});
		$('#' + $(this).attr('id')).on('rowDoubleClick', function (event) {
			var item = $(this)
				.next(".context-menus:first")
				.find(".context-menu-" + event.args.index + " ul li b a");

			if (item.length > 0) { document.location = item.attr("href"); return; }

			item = $(this)
               .next(".context-menus:first")
               .find(".context-menu-" + event.args.index + " ul li b input[type=button].enable-double-click");

			if (item.length > 0) { item.click(); return; }
		});
		$("table tr td").each(function () {
			$(this).css("white-space", "pre-line");
		});
	});
	$(obj).find(".data-tabs").each(function () { $(this).jqxTabs({ theme: 'orange', width: (window.innerWidth - 300) }); });
	$(obj).find(".operation-tabs").each(function () { $(this).jqxTabs({ theme: 'orange', animationType: 'fade', width: (window.innerWidth - 300) }); });
	$(obj).find(".operation-tabs").each(function () {
		$(this).find(".operation-form").each(function (i, e) {
			if ($(this).hasClass("disabled")) {
				$(this).closest(".operation-tabs").jqxTabs("disableAt", i);
			}
		});
	});

	$(obj).find(".collapsible-tabs").each(function () {
		$(this).jqxTabs({ collapsible: true });
		$(this).jqxTabs("collapse");
	});
	$(obj).find(".operation-tabs").on("tabclick", function (event) {
		if (event.args.item == $(this).jqxTabs('selectedItem')) {
			return;
		}
		$(this).jqxTabs('expand');
	});
	$(obj).find(".operation-tabs").each(function () {
		if ($(this).closest('field-set').hasClass("search-data")) {
			$(this).css("width", "900px");
		}
	});
	$(obj).find('.search-data .operation-form').each(function () {

		$(this).ajaxForm({
			dataType: 'html',
			beforeSubmit: function (arr, $form, options) {
				$form.closest("fieldset.search-data").children("div.search-result").empty();
			},
			success: function (data, statusText, xhr, $form) {
				$form.closest("fieldset.search-data").children("div.search-result").html(data);
				render($form.closest("fieldset.search-data"));
			},
			error: function (req, status, error, $form) {
				alert(error);
			}
		});
	});


	$(obj).find(".context-menu").each(function () { $(this).jqxMenu({ mode: "vertical", width: "160px" }); });
	$(obj).find(".context-menu").on("mouseleave", function (e) { $(this).fadeOut(50); });

	$('input[data-type="s-boolean"][type="checkbox"]').change(function () {
		$('input[name="' + $(this).attr("name") + '"][type="hidden"]').attr("disabled", $(this).is(":checked"));
	});
	$('input[data-type="c-fat-string"]').each(function () {

		if (!$(this).closest('dd').has('textarea').length) {
			$(this).after(
            $("<textarea/>")
                .attr('name', $(this).attr("name")).val($(this).attr("value"))
        );
		}
		$(this).css("display", "none");
	});
	$('textarea').change(function () {
		$(this).prev('input[data-type="c-fat-string"]')
            .val($(this).val());
	});
	$('input[data-type="c-phone"]').each(function () {

	});
	$('input[data-type="c-password"]').each(function () {
		$(this).attr("type", "password");
	});

	$('input[data-type="s-decimal"]').each(function () {
		$(this).val(0.00);
		$(this).keypress(function (key) {
			if (key.charCode == 0) return true;
			if (key.charCode == 190) return true;
			if (key.charCode == 44) return true;
			if (key.charCode < 48 || key.charCode > 57) return false;
			return true;
		});
	});

	$('input[data-type="s-int-32"]').each(function () {
		$(this).val(0);
		$(this).keypress(function (key) {
			if (key.charCode == 0) return true;
			if (key.charCode < 48 || key.charCode > 57) return false;
			return true;
		});
	});
	$('dd.link').each(function () {
		var filePath = $(this).children('span').children('span').children('span').children('span').html();
		var correctedPath = "file:///" + filePath.replace(/\\/g, "/");
		$(this).children('span').after('<form target="_blank" name="input" action="/File/Open/" method="get"> ' +
	        '<a id="view" href="javascript:;" onclick="parentNode.submit();">View</a>' +
	        '<input id="formpath" type="hidden" name="filePath" value="' + filePath + '"/>' +
	        '</form>');
		$(this).children('span').children('span').remove();
	});
	$('#formpath').each(function () {
		if ($(this).val() == null || $(this).val().length <= 0) {
			$('#view').before(this).click(function (e) {
				e.preventDefault();
			});
		}
	});

	configureValidator([
            {
            	datatype: 's-string',
            	regex: '',
            	errorMessage: 'en az 3 karakter olmalý'
            },
            //{
            //    datatype: 's-int-32',
            //    regex: '^-?\d+\.?\d*$',
            //    errorMessage: 'en az 2 en fazla 4 karakter olmalý'
            //},
            {
            	datatype: 's-date-time',
            	regex: '',
            	errorMessage: "Tarih 01.01.1990'dan daha önce olamaz"
            },
            {
            	datatype: 's-decimal',
            	regex: '',
            	errorMessage: '-10000.0000 ile 10000.0000 arasýnda olmalý'
            },
            {
            	datatype: 'c-phone',
            	regex: '[0-9]{10,}$',
            	errorMessage: '10 haneli olmalý ve alan kodu en küçük 2 ile baþlamalý'
            }

	]);
};

function selectRow(btn) {
	var modal = $(btn).closest('div.modal-content').parent();
	var input = $("input[data-return=" + modal.attr('id') + "]");
	input.val($(btn).next().val());
	input.next().val($(btn).next().next().val());
	modal.jqxWindow('close');
}

function confirmDialog(message, okText, form) {
	var buttons = new Object();
	buttons[okText] = function () { $(this).dialog("close"); $(this).dialog("destroy"); form.submit(); };
	buttons["Cancel"] = function () { $(this).dialog("close"); $(this).dialog("destroy"); };
	$('<div>' + message + '</div>').dialog({
		modal: true,
		title: "Confirmation",
		buttons: buttons
	});
}
