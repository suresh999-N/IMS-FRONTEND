import{r as e}from"./rolldown-runtime-QTnfLwEv.js";import{h as t,t as n}from"./jsx-runtime-Md2kjDfB.js";import{c as r,i,s as a,t as o,u as s}from"./erp-D2tD2v1L.js";import{t as c}from"./calendar-days-BX12kxgl.js";import{n as l,t as u}from"./Payments-JjuknDak.js";import{t as d}from"./pencil-EgVeGJB0.js";import{t as f}from"./printer-jNtDF2O3.js";import{t as p}from"./trash-2-Ykn9Tzs3.js";import{An as m,C as h,Gn as g,Mn as _,N as v,Nn as y,Pn as b,Q as x,V as ee,X as te,Y as S,et as C,g as w,h as T,ir as ne,it as E,j as D,jn as O,u as k,w as A,z as re}from"./index-BN9BF01H.js";import{t as ie}from"./CurrencyInput-CNIwNKqq.js";import{t as ae}from"./InputField-DsW-oyi4.js";import{t as j}from"./SearchableSelect-BVllMUO4.js";import{t as oe}from"./FormModal-BclTzECm.js";import{E as se,_ as ce,a as le,p as M,t as ue}from"./businessApi-DAnQW-6A.js";import{t as de}from"./DatePicker-BO9DgDBN.js";import{n as fe,t as pe}from"./jspdf.plugin.autotable-DE-yZsGN.js";var N=e(t(),1),P=n(),me=[{value:`Cash`,label:`Cash`},{value:`Bank Transfer`,label:`Bank Transfer`},{value:`UPI`,label:`UPI`},{value:`Card`,label:`Card`},{value:`Cheque`,label:`Cheque`}],he=[{value:`all`,label:`All Payments`},{value:`success`,label:`Success`},{value:`pending`,label:`Pending`},{value:`failed`,label:`Failed`},{value:`cancelled`,label:`Cancelled`},{value:`reversed`,label:`Reversed`},{value:`month`,label:`This Month`},{value:`last30`,label:`Last 30 Days`},{value:`large`,label:`Large Payments`}],ge=5e4,_e=`StockPilot IMS`,ve=`IMS`,F={name:_e,address:``,email:``,phone:``,gstNumber:``},ye=595.28,be=841.89,I=794,L=1123,xe=[`paymentNumber`,`paymentDate`,`partyName`,`invoiceNumber`,`invoiceStatus`,`amount`,`status`,`actions`],Se=[`paymentNumber`,`actions`],Ce=`v12`,we={customer:`ims.customerPayments.visibleColumns.${Ce}`,supplier:`ims.supplierPayments.visibleColumns.${Ce}`},Te={paymentNumber:144,paymentDate:112,partyName:160,invoiceNumber:140,poId:180,invoiceStatus:100,amount:112,paymentMethod:132,referenceNumber:160,status:96,createdBy:130,notes:220,cancelledAt:140,cancellationReason:220,actions:64};function Ee(e,t){if(e==null||e===``)return null;let n=Number(e);if(!Number.isInteger(n)||n<=0)throw Error(`${t} must be selected from live API records.`);return n}function R(e){return e.paymentNumber?e.paymentNumber:`PAY-${String(e.paymentDate||b()).replaceAll(`-`,``)}-${String(e.paymentId||e.id||0).padStart(3,`0`)}`}function z(e){return String(e?.paymentRowId||e?.id||e?.paymentId||R(e))}function B(e){let t=e?.paymentId||e?.id;if(t!=null&&String(t).trim())return`payment-${String(t).trim()}`;let n=R(e);return n&&!n.endsWith(`-000`)?`number-${n}`:[`payment`,e?.invoiceId||`no-invoice`,e?.paymentDate||`no-date`,e?.amount||0,e?.referenceNumber||`no-reference`,e?.partyName||`no-party`].map(e=>String(e).trim().toLowerCase().replace(/\s+/g,`-`)).join(`|`)}function De(e){return{...e,paymentRowId:B(e)}}function V(e){let t=String(e.status||``).trim().toLowerCase();return t===`cancelled`||t===`canceled`?`Cancelled`:t===`reversed`||t===`voided`?`Reversed`:t===`failed`?`Failed`:t===`completed`||t===`success`||t===`paid`||t===`reconciled`||t===`posted`||t===`received`?`Completed`:`Pending`}function H(e){let t=V({status:e});return t===`Completed`?{label:`Success`,type:`success`,icon:C}:t===`Cancelled`?{label:`Cancelled`,type:`cancelled`,icon:l}:t===`Reversed`?{label:`Reversed`,type:`draft`,icon:re}:t===`Failed`?{label:`Failed`,type:`failed`,icon:l}:{label:`Pending`,type:`pending`,icon:c}}function Oe({status:e}){let t=H(e);return(0,P.jsx)(o,{type:t.type,icon:t.icon,children:t.label})}function ke(e){let t=Number(e||0),n=Math.abs(t),r=t<0?`-`:``;return n>=1e7?`${r}₹${(n/1e7).toFixed(1)} Cr`:n>=1e5?`${r}₹${(n/1e5).toFixed(1)} L`:m(t)}function Ae(e){let t=String(e||``).trim().toLowerCase();return t===`draft`||t===`sent`||t===`unpaid`?`Unpaid`:t===`partially paid`||t===`partial`?`Partial`:t===`paid`?`Paid`:t===`overdue`?`Overdue`:t===`cancelled`||t===`canceled`?`Cancelled`:e?String(e).trim():`Unpaid`}function je(e){let t=Ae(e);return t===`Paid`?{label:`Paid`,type:`success`,icon:C}:t===`Partial`?{label:`Partial`,type:`received`,icon:x}:t===`Unpaid`?{label:`Unpaid`,type:`pending`,icon:c}:t===`Overdue`?{label:`Overdue`,type:`failed`,icon:D}:t===`Cancelled`?{label:`Cancelled`,type:`cancelled`,icon:l}:{label:t,type:`info`,icon:S}}function Me({status:e}){let t=je(e);return(0,P.jsx)(o,{type:t.type,icon:t.icon,children:t.label})}function Ne(e,t,n={}){if(!e||t===`all`)return!0;let r=new Date(`${e}T00:00:00`);if(Number.isNaN(r.getTime()))return!0;let i=new Date;if(i.setHours(0,0,0,0),t===`today`)return r.getTime()===i.getTime();if(t===`last7`||t===`last30`){let e=t===`last7`?7:30,n=new Date(i);return n.setDate(i.getDate()-e+1),r>=n&&r<=i}if(t===`month`)return e.startsWith(i.toISOString().slice(0,7));if(t===`custom`){let e=n.from?new Date(`${n.from}T00:00:00`):null,t=n.to?new Date(`${n.to}T23:59:59`):null;if(e&&r<e||t&&r>t)return!1}return!0}function U(e){return String(e??``).replaceAll(`&`,`&amp;`).replaceAll(`<`,`&lt;`).replaceAll(`>`,`&gt;`).replaceAll(`"`,`&quot;`).replaceAll(`'`,`&apos;`)}function Pe(e){return String(e||`Receipt`).replace(/[\\/:*?"<>|]+/g,`-`).replace(/\s+/g,`-`).replace(/-+/g,`-`)}async function W(e){let t=String(e||``).trim();if(!t)return!1;if(typeof navigator<`u`&&navigator.clipboard?.writeText)return await navigator.clipboard.writeText(t),!0;if(typeof document>`u`)return!1;let n=document.createElement(`textarea`);n.value=t,n.setAttribute(`readonly`,``),n.style.position=`fixed`,n.style.top=`-1000px`,document.body.appendChild(n),n.select();try{return document.execCommand(`copy`)}finally{document.body.removeChild(n)}}function Fe(){return new Date().toLocaleString(`en-IN`,{year:`numeric`,month:`short`,day:`2-digit`,hour:`2-digit`,minute:`2-digit`})}function G(e){return new Intl.NumberFormat(`en-IN`,{style:`currency`,currency:`INR`,minimumFractionDigits:2,maximumFractionDigits:2}).format(Number(e||0))}function Ie(e){if(!e)return`N/A`;let t=new Date(`${e}T00:00:00`);return Number.isNaN(t.getTime())?O(e)||`N/A`:t.toLocaleDateString(`en-GB`,{day:`2-digit`,month:`short`,year:`numeric`}).replaceAll(` `,`-`)}function Le(){return[F.address,F.email?`Email: ${F.email}`:``,F.phone?`Phone: ${F.phone}`:``,F.gstNumber?`GST: ${F.gstNumber}`:``].filter(Boolean)}function Re(e){let t=Ae(e);return t===`Paid`?`badge-green`:t===`Partial`?`badge-amber`:`badge-red`}function ze(e){let t=V({status:e});return t===`Completed`?`badge-green`:t===`Pending`?`badge-amber`:`badge-red`}function K(e,t=44,n=2){let r=String(e||`-`).split(/\s+/).filter(Boolean).flatMap(e=>{if(e.length<=t)return[e];let n=[];for(let r=0;r<e.length;r+=t)n.push(e.slice(r,r+t));return n}),i=[],a=``;r.forEach(e=>{let n=a?`${a} ${e}`:e;if(n.length>t&&a){i.push(a),a=e;return}a=n}),a&&i.push(a);let o=i.slice(0,n);return i.length>n&&(o[n-1]=`${o[n-1].slice(0,Math.max(0,t-1)).trim()}...`),o.length>0?o:[`-`]}function Be(e,t,n,r={}){let{maxLength:i=44,maxLines:a=2,lineHeight:o=18,className:s=`value`,anchor:c=`start`}=r;return`
    <text x="${t}" y="${n}" class="${s}" text-anchor="${c}">
      ${K(e,i,a).map((e,n)=>`<tspan x="${t}" dy="${n===0?0:o}">${U(e)}</tspan>`).join(``)}
    </text>
  `}function Ve(e,t,n){if(!Array.isArray(e)||e.length===0)return;let r=new fe({orientation:`landscape`,unit:`mm`,format:`a4`}),i=t?`Supplier Payments Executive Statement`:`Customer Payments Executive Statement`,a=t?`Supplier Name`:`Customer Name`,o=t?`PO Number`:`Invoice Number`,s=e.length,c=e.reduce((e,t)=>e+(Number(t.amount)||0),0),l=e.filter(e=>{let t=H(e.status).label;return t===`Success`||t===`Completed`}).length,u=e.filter(e=>H(e.status).label===`Pending`).length;r.setFillColor(15,23,42),r.roundedRect(12,12,273,22,3,3,`F`),r.setFillColor(255,255,255),r.roundedRect(16,15.5,15,15,3,3,`F`),r.setTextColor(15,23,42),r.setFont(`helvetica`,`bold`),r.setFontSize(10),r.text(`IMS`,23.5,25,{align:`center`}),r.setFontSize(13),r.setTextColor(255,255,255),r.text(`StockPilot IMS`,35,22.5),r.setFontSize(8.5),r.setFont(`helvetica`,`normal`),r.setTextColor(148,163,184),r.text(i,35,28),r.setFontSize(8),r.setFont(`helvetica`,`bold`),r.setTextColor(t?45:96,t?212:165,t?191:250),r.text(`OFFICIAL FINANCIAL RECORD`,279,21.5,{align:`right`}),r.setTextColor(255,255,255),r.setFontSize(8),r.setFont(`helvetica`,`normal`),r.text(`Generated: ${Fe()}`,279,28,{align:`right`}),[{label:`TOTAL RECORDS`,val:`${s} Payments`,color:[15,23,42]},{label:`TOTAL SETTLED AMOUNT`,val:G(c),color:[16,185,129]},{label:`COMPLETED / SUCCESS`,val:`${l}`,color:[22,163,74]},{label:`PENDING RECONCILIATION`,val:`${u}`,color:[217,119,6]}].forEach((e,t)=>{let n=12+t*69.25;r.setFillColor(248,250,252),r.setDrawColor(226,232,240),r.roundedRect(n,38,65.25,14,2,2,`FD`),r.setFontSize(7),r.setFont(`helvetica`,`bold`),r.setTextColor(100,116,139),r.text(e.label,n+4,43),r.setFontSize(9.5),r.setFont(`helvetica`,`bold`),r.setTextColor(e.color[0],e.color[1],e.color[2]),r.text(e.val,n+4,49)}),pe(r,{startY:56,margin:{left:12,right:12,bottom:16},head:[[`Payment No`,`Date`,a,o,`Amount (INR)`,`Payment Method`,`Reference / UTR`,`Status`]],body:e.map(e=>[R(e),Ie(e.paymentDate),e.partyName||(t?`Supplier`:`Customer`),t?e.poNumber||(e.poId?`PO-${String(e.poId).padStart(3,`0`)}`:`-`):e.invoiceNumber||(e.invoiceId?`INV-${String(e.invoiceId).padStart(3,`0`)}`:`-`),G(e.amount),e.paymentMethod||`Bank Transfer`,e.referenceNumber||`N/A`,H(e.status).label]),theme:`grid`,headStyles:{fillColor:[15,23,42],textColor:[255,255,255],fontStyle:`bold`,fontSize:8.5,halign:`left`,cellPadding:2.5},bodyStyles:{fontSize:8,textColor:[30,41,59],cellPadding:2.5,overflow:`linebreak`},columnStyles:{0:{fontStyle:`bold`,cellWidth:36},1:{cellWidth:26},2:{cellWidth:54},3:{cellWidth:34},4:{halign:`right`,fontStyle:`bold`,cellWidth:32},5:{cellWidth:30},6:{cellWidth:35},7:{halign:`center`,cellWidth:26}},alternateRowStyles:{fillColor:[248,250,252]},didDrawPage:e=>{let t=r.internal.getNumberOfPages();r.setFontSize(8),r.setFont(`helvetica`,`normal`),r.setTextColor(148,163,184),r.text(`Generated by StockPilot IMS  |  Page ${e.pageNumber} of ${t}`,297/2,204,{align:`center`})}});let d=new Date().toISOString().slice(0,10),f=t?`Supplier-Payments-Report`:`Customer-Payments-Report`;r.save(`${f}-${d}.pdf`)}function He({payment:e,invoice:t,metrics:n,generatedBy:r}){let i=H(e.status).label,a=je(e.invoiceStatus).label,o=R(e),s=q(e,t),c=Fe(),l=Le(),u=Ie(t?.invoiceDate||e.invoiceDate||e.paymentDate),d=Number(n.outstandingAfter||0),f=e.partyName||`Customer`,p=String(r||`System Administrator`).slice(0,34);return`
    <svg xmlns="http://www.w3.org/2000/svg" width="${I}" height="${L}" viewBox="0 0 ${I} ${L}">
      <defs>
        <linearGradient id="headerGrad" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="#0f172a"/>
          <stop offset="50%" stop-color="#1e293b"/>
          <stop offset="100%" stop-color="#2563eb"/>
        </linearGradient>
      </defs>

      <style>
        .page-bg { fill: #f8fafc; }
        .sheet-bg { fill: #ffffff; stroke: #e2e8f0; stroke-width: 1.5; }
        .section-title { fill: #475569; font: 800 11px Arial, sans-serif; letter-spacing: 1.2px; text-transform: uppercase; }
        .card-bg { fill: #f8fafc; stroke: #e2e8f0; stroke-width: 1; }
        .card-label { fill: #64748b; font: 700 10px Arial, sans-serif; letter-spacing: 0.8px; text-transform: uppercase; }
        .card-val { fill: #0f172a; font: 700 13px Arial, sans-serif; }
        .card-val-large { fill: #0f172a; font: 800 14px Arial, sans-serif; }
        
        .brand-name { fill: #ffffff; font: 800 20px Arial, sans-serif; letter-spacing: -0.2px; }
        .brand-sub { fill: #94a3b8; font: 500 11px Arial, sans-serif; }
        .brand-line { fill: #cbd5e1; font: 500 11px Arial, sans-serif; }

        .meta-tag { fill: #60a5fa; font: 800 11px Arial, sans-serif; letter-spacing: 1.2px; }
        .meta-lbl { fill: #94a3b8; font: 700 10px Arial, sans-serif; letter-spacing: 0.6px; }
        .meta-txt { fill: #ffffff; font: 800 12px Consolas, monospace; }

        .stamp-bg { fill: #f0fdf4; stroke: #bbf7d0; stroke-width: 1; }
        .stamp-circle { fill: #16a34a; }
        .stamp-check { fill: #ffffff; font: 900 12px Arial, sans-serif; }
        .stamp-text { fill: #166534; font: 800 12px Arial, sans-serif; letter-spacing: 0.8px; }

        .table-card { fill: #ffffff; stroke: #e2e8f0; stroke-width: 1; }
        .table-head { fill: #f1f5f9; stroke: #e2e8f0; stroke-width: 1; }
        .head-txt { fill: #475569; font: 800 11px Arial, sans-serif; letter-spacing: 0.8px; text-transform: uppercase; }
        .row-line { stroke: #f1f5f9; stroke-width: 1; }
        .cell-desc { fill: #334155; font: 600 13px Arial, sans-serif; }
        .cell-amt { fill: #0f172a; font: 700 13px Consolas, monospace; }

        .current-bg { fill: #f0fdf4; }
        .current-bar { fill: #10b981; }
        .current-desc { fill: #166534; font: 800 13.5px Arial, sans-serif; }
        .current-amt { fill: #15803d; font: 800 14px Consolas, monospace; }

        .balance-open-bg { fill: #fff7ed; }
        .balance-open-bar { fill: #f97316; }
        .balance-open-desc { fill: #9a3412; font: 800 13.5px Arial, sans-serif; }
        .balance-open-amt { fill: #c2410c; font: 800 14px Consolas, monospace; }

        .balance-paid-bg { fill: #f0fdf4; }
        .balance-paid-bar { fill: #10b981; }
        .balance-paid-desc { fill: #166534; font: 800 13.5px Arial, sans-serif; }
        .balance-paid-amt { fill: #15803d; font: 800 14px Consolas, monospace; }

        .badge-bg-green { fill: #dcfce7; stroke: #86efac; stroke-width: 1; }
        .badge-txt-green { fill: #15803d; font: 800 11px Arial, sans-serif; }
        .badge-bg-amber { fill: #fff7ed; stroke: #fed7aa; stroke-width: 1; }
        .badge-txt-amber { fill: #c2410c; font: 800 11px Arial, sans-serif; }
        .badge-bg-red { fill: #fef2f2; stroke: #fca5a5; stroke-width: 1; }
        .badge-txt-red { fill: #b91c1c; font: 800 11px Arial, sans-serif; }

        .sig-line { stroke: #cbd5e1; stroke-width: 1.5; stroke-dasharray: 4 3; }
        .sig-lbl { fill: #64748b; font: 700 11px Arial, sans-serif; letter-spacing: 0.6px; text-transform: uppercase; }
        .seal-bg { fill: #ffffff; stroke: #cbd5e1; stroke-width: 2; stroke-dasharray: 3 3; }
        .seal-txt { fill: #94a3b8; font: 800 9px Arial, sans-serif; }

        .footer-note { fill: #64748b; font: 500 11px Arial, sans-serif; }
        .footer-box { fill: #f8fafc; stroke: #e2e8f0; stroke-width: 1; }
        .footer-meta { fill: #94a3b8; font: 600 11px Arial, sans-serif; }
      </style>

      <rect class="page-bg" width="${I}" height="${L}"/>
      <rect class="sheet-bg" x="32" y="32" width="730" height="1059" rx="16"/>

      <!-- Header Banner -->
      <rect x="52" y="52" width="690" height="100" rx="12" fill="url(#headerGrad)"/>
      <rect x="72" y="72" width="56" height="56" rx="12" fill="rgba(255,255,255,0.18)" stroke="rgba(255,255,255,0.3)"/>
      <text x="100" y="107" text-anchor="middle" fill="#ffffff" font-weight="900" font-size="20" letter-spacing="1">IMS</text>
      
      <text x="142" y="85" class="brand-name">${U(F.name||_e)}</text>
      <text x="142" y="101" class="brand-sub">Inventory &amp; Financial Management System</text>
      ${l.slice(0,1).map(e=>`<text x="142" y="116" class="brand-line">${U(K(e,40,1)[0])}</text>`).join(``)}

      <rect x="490" y="66" width="232" height="26" rx="13" fill="rgba(96,165,250,0.18)" stroke="rgba(96,165,250,0.4)"/>
      <text x="606" y="83" text-anchor="middle" class="meta-tag">${U(`CUSTOMER PAYMENT RECEIPT`)}</text>

      <text x="550" y="110" text-anchor="end" class="meta-lbl">RECEIPT NO:</text>
      <text x="558" y="110" class="meta-txt">${U(o)}</text>
      <text x="550" y="128" text-anchor="end" class="meta-lbl">DATE:</text>
      <text x="558" y="128" class="meta-txt">${U(Ie(e.paymentDate))}</text>

      <!-- Watermark Stamp -->
      <rect class="stamp-bg" x="52" y="168" width="690" height="38" rx="8"/>
      <circle class="stamp-circle" cx="76" cy="187" r="10"/>
      <text class="stamp-check" x="76" y="191" text-anchor="middle">✓</text>
      <text class="stamp-text" x="96" y="191">OFFICIAL SETTLEMENT RECEIPT — RECORDED</text>

      <!-- Section 1: Details Card -->
      <text class="section-title" x="52" y="232">${U(`CUSTOMER`)} &amp; DETAILS</text>
      <rect class="card-bg" x="52" y="244" width="690" height="92" rx="10"/>
      <line x1="224" y1="244" x2="224" y2="336" stroke="#e2e8f0"/>
      <line x1="396" y1="244" x2="396" y2="336" stroke="#e2e8f0"/>
      <line x1="568" y1="244" x2="568" y2="336" stroke="#e2e8f0"/>

      <text class="card-label" x="70" y="268">${U(`Customer`)} Name</text>
      ${Be(f,70,292,{maxLength:20,maxLines:2,className:`card-val-large`})}

      <text class="card-label" x="242" y="268">${U(`Invoice Number`)}</text>
      <text class="card-val" x="242" y="292">${U(s)}</text>

      <text class="card-label" x="414" y="268">Date</text>
      <text class="card-val" x="414" y="292">${U(u)}</text>

      <text class="card-label" x="586" y="268">Status</text>
      <rect class="${Re(a)}" x="582" y="278" width="124" height="26" rx="13"/>
      <text class="${Re(a).replace(`badge-`,`badge-txt-`)}" x="644" y="295" text-anchor="middle">${U(a)}</text>

      <!-- Section 2: Financial Breakdown Table -->
      <text class="section-title" x="52" y="362">FINANCIAL SETTLEMENT BREAKDOWN</text>
      <rect class="table-card" x="52" y="374" width="690" height="220" rx="10"/>
      <rect class="table-head" x="52" y="374" width="690" height="36" rx="10"/>
      <text class="head-txt" x="72" y="397">Description</text>
      <text class="head-txt" x="722" y="397" text-anchor="end">Amount</text>

      <!-- Table Rows -->
      <line class="row-line" x1="52" y1="410" x2="742" y2="410"/>
      <text class="cell-desc" x="72" y="434">${U(`Invoice Total Amount`)}</text>
      <text class="cell-amt" x="722" y="434" text-anchor="end">${U(G(n.invoiceTotal))}</text>

      <line class="row-line" x1="52" y1="455" x2="742" y2="455"/>
      <text class="cell-desc" x="72" y="479">Previous Cumulative Payments</text>
      <text class="cell-amt" x="722" y="479" text-anchor="end">${U(G(n.previousPayments))}</text>

      <!-- Current Payment Row -->
      <rect class="current-bg" x="53" y="500" width="688" height="46"/>
      <rect class="current-bar" x="53" y="500" width="5" height="46"/>
      <text class="current-desc" x="72" y="528">Current Payment Settled</text>
      <text class="current-amt" x="722" y="528" text-anchor="end">${U(G(e.amount))}</text>

      <!-- Remaining Balance Row -->
      <rect class="${d>0?`balance-open-bg`:`balance-paid-bg`}" x="53" y="547" width="688" height="46"/>
      <rect class="${d>0?`balance-open-bar`:`balance-paid-bar`}" x="53" y="547" width="5" height="46"/>
      <text class="${d>0?`balance-open-desc`:`balance-paid-desc`}" x="72" y="575">Remaining Outstanding Balance</text>
      <text class="${d>0?`balance-open-amt`:`balance-paid-amt`}" x="722" y="575" text-anchor="end">${U(G(n.outstandingAfter))}</text>

      <!-- Section 3: Payment Method & Details -->
      <text class="section-title" x="52" y="622">PAYMENT METHOD &amp; EXECUTION</text>
      <rect class="card-bg" x="52" y="634" width="690" height="84" rx="10"/>
      <line x1="282" y1="634" x2="282" y2="718" stroke="#e2e8f0"/>
      <line x1="512" y1="634" x2="512" y2="718" stroke="#e2e8f0"/>

      <text class="card-label" x="70" y="658">Payment Method</text>
      <text class="card-val" x="70" y="682">${U(e.paymentMethod||`Bank Transfer`)}</text>

      <text class="card-label" x="300" y="658">Reference / UTR Number</text>
      <text class="card-val" x="300" y="682">${U(e.referenceNumber||`N/A`)}</text>

      <text class="card-label" x="530" y="658">Payment Status</text>
      <rect class="${ze(e.status)}" x="530" y="668" width="124" height="26" rx="13"/>
      <text class="${ze(e.status).replace(`badge-`,`badge-txt-`)}" x="592" y="685" text-anchor="middle">${U(i)}</text>

      <!-- Section 4: Signature & Stamp -->
      <text class="section-title" x="52" y="746">AUTHORIZATION &amp; SIGNATURE</text>
      <line class="sig-line" x1="72" y1="810" x2="350" y2="810"/>
      <text class="sig-lbl" x="211" y="830" text-anchor="middle">Customer Signature</text>

      <circle class="seal-bg" cx="560" cy="790" r="22"/>
      <text class="seal-txt" x="560" y="793" text-anchor="middle">SEAL</text>
      <line class="sig-line" x1="440" y1="810" x2="720" y2="810"/>
      <text class="sig-lbl" x="580" y="830" text-anchor="middle">Authorized Signatory</text>

      <!-- Footer Audit -->
      <line x1="52" y1="880" x2="742" y2="880" stroke="#e2e8f0"/>
      <text class="footer-note" x="397" y="904" text-anchor="middle">This is a computer-generated official payment receipt issued by StockPilot IMS. Valid without physical signature.</text>
      
      <rect class="footer-box" x="52" y="920" width="690" height="34" rx="6"/>
      <text class="footer-meta" x="70" y="941">Generated On: ${U(c)}</text>
      <text class="footer-meta" x="397" y="941" text-anchor="middle">Issued By: ${U(p)}</text>
      <text class="footer-meta" x="724" y="941" text-anchor="end">System: ${U(ve)}</text>
    </svg>
  `}function Ue(e){let[,t=``]=e.split(`,`);return atob(t)}function We(e){let t=Ue(e),n=`q\n${ye} 0 0 ${be} 0 0 cm\n/Im1 Do\nQ\n`,r=[`<< /Type /Catalog /Pages 2 0 R >>`,`<< /Type /Pages /Kids [3 0 R] /Count 1 >>`,`<< /Type /Page /Parent 2 0 R /MediaBox [0 0 ${ye} ${be}] /Resources << /XObject << /Im1 4 0 R >> >> /Contents 5 0 R >>`,`<< /Type /XObject /Subtype /Image /Width ${I} /Height ${L} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length ${t.length} >>\nstream\n${t}\nendstream`,`<< /Length ${n.length} >>\nstream\n${n}endstream`],i=`%PDF-1.4
`,a=[0];r.forEach((e,t)=>{a.push(i.length),i+=`${t+1} 0 obj\n${e}\nendobj\n`});let o=i.length;i+=`xref\n0 ${r.length+1}\n0000000000 65535 f \n`,a.slice(1).forEach(e=>{i+=`${String(e).padStart(10,`0`)} 00000 n \n`}),i+=`trailer\n<< /Size ${r.length+1} /Root 1 0 R >>\nstartxref\n${o}\n%%EOF`;let s=new Uint8Array(i.length);for(let e=0;e<i.length;e+=1)s[e]=i.charCodeAt(e)&255;return new Blob([s],{type:`application/pdf`})}async function Ge(e){let t=new Image,n=new Blob([e],{type:`image/svg+xml;charset=utf-8`}),r=URL.createObjectURL(n);try{await new Promise((e,n)=>{t.onload=e,t.onerror=n,t.src=r});let e=document.createElement(`canvas`);e.width=I,e.height=L;let n=e.getContext(`2d`);return n.fillStyle=`#ffffff`,n.fillRect(0,0,e.width,e.height),n.drawImage(t,0,0,e.width,e.height),e.toDataURL(`image/jpeg`,.96)}finally{URL.revokeObjectURL(r)}}async function Ke({payment:e,invoice:t,allPayments:n,generatedBy:r}){if(typeof document>`u`||typeof Image>`u`)throw Error(`PDF generation is only available in the browser.`);let i=We(await Ge(He({payment:e,invoice:t,metrics:Qe(e,t,n),generatedBy:r}))),a=URL.createObjectURL(i),o=document.createElement(`a`),s=Pe(R(e));o.href=a,o.download=`Customer-Payment-Receipt-${s}.pdf`,o.click(),URL.revokeObjectURL(a)}function qe(e){let t=String(e||``).trim();return!t||/unknown column|unknown table|sql|mysql|database|exception|stack|system\.|microsoft\./i.test(t)?`Unable to load payment data right now.`:t}function Je(e){return String(e?.invoiceId||e?.id||``)}function q(e,t){return e.invoiceNumber||t?.invoiceNumber||(e.invoiceId?`INV-${String(e.invoiceId).padStart(3,`0`)}`:`Not applied`)}function Ye(e,t){let n=Number(e.invoiceAmount||t?.totalAmount||0),r=Number(e.outstandingAfter||t?.balanceAmount||0),i=Number(e.outstandingBefore||r+Number(e.amount||0));return{invoiceTotal:n,alreadyPaid:Math.max(0,n-Number(t?.balanceAmount||0)),outstandingBefore:i,outstandingAfter:Math.max(0,r)}}function Xe(e){let t=e?.paymentId??e?.id,n=Number(t);if(Number.isFinite(n)&&n>0)return n;let r=String(R(e)).match(/(\d+)(?!.*\d)/);return r?Number(r[1]):0}function J(e){return[e?.createdAt||e?.paymentDate||``,e?.paymentDate||``,String(Xe(e)).padStart(12,`0`),R(e)].join(`|`)}function Ze(e,t,n){let r=String(t?.invoiceId||n?.invoiceId||n?.id||``).trim(),i=String(e?.invoiceId||``).trim();if(r&&i)return i===r;let a=q(t,n),o=q(e,null);return a!==`Not applied`&&o===a}function Y(e){let t=V(e);return t!==`Cancelled`&&t!==`Failed`&&t!==`Reversed`}function Qe(e,t,n=[]){let r=Ye(e,t),i=J(e),a=z(e),o=n.filter(n=>z(n)!==a&&Y(n)&&Ze(n,e,t)&&J(n)<i).reduce((e,t)=>e+Number(t.amount||0),0),s=Number(e.amount||0),c=Math.max(0,r.invoiceTotal-o-s);return{...r,previousPayments:o,outstandingAfter:c}}function X(e,t,n=[]){return n.filter(n=>Y(n)&&Ze(n,e,t)).sort((e,t)=>J(e).localeCompare(J(t)))}function Z(e,t){return Number(e.outstandingAfter||0)<=0?`paid`:Ae(t)===`Overdue`?`overdue`:`partial`}function Q(e,t){let n=Number(e.invoiceTotal||0);if(n<=0)return 0;let r=Math.min(n,Number(e.previousPayments||0)+Number(t.amount||0));return Math.round(r/n*100)}function $e(e){try{if(typeof window>`u`)return null;let t=window.localStorage.getItem(e),n=t?JSON.parse(t):null;return Array.isArray(n)?n.filter(Boolean):null}catch{return null}}function $({label:e,value:t,wide:n=!1}){return(0,P.jsxs)(`div`,{className:`payment-detail-item ${n?`payment-detail-item--wide`:``}`.trim(),children:[(0,P.jsx)(`dt`,{children:e}),(0,P.jsx)(`dd`,{children:t||`-`})]})}function et({payment:e,invoice:t,allPayments:n=[],onClose:r,onPrint:i,onDownloadReceipt:a,onOpenInvoice:o}){let[s,c]=(0,N.useState)(e),[l,d]=(0,N.useState)(``);(0,N.useEffect)(()=>{c(e)},[e]),(0,N.useEffect)(()=>{if(!l)return;let e=window.setTimeout(()=>{d(``)},1200);return()=>window.clearTimeout(e)},[l]);let p=V(s),g=Ae(s.invoiceStatus),_=Qe(s,t,n),v=X(s,t,n),y=Z(_,g),b=Q(_,s),x=Number(_.invoiceTotal||0),ee=Math.max(0,Math.min(x,x-Number(_.outstandingAfter||0))),te=v.reduce((e,t)=>e+Number(t.amount||0),0),C=z(s),w=q(s,t),T=s.referenceNumber||``,ne=[{label:`Invoice Total`,value:m(_.invoiceTotal)},{label:`Previous Payments`,value:m(_.previousPayments)},{label:`Current Payment`,value:m(s.amount),emphasis:!0},{label:`Remaining Balance`,value:m(_.outstandingAfter),strong:!0,tone:y}],E=async(e,t)=>{try{await W(e)&&(d(t),h({type:`success`,title:`Receipt`,message:`Copied to clipboard`}))}catch{h({type:`error`,title:`Receipt`,message:`Unable to copy value.`})}};return(0,P.jsxs)(`div`,{className:`payment-drawer payment-drawer--landscape`,role:`dialog`,"aria-modal":`true`,"aria-labelledby":`payment-drawer-title`,children:[(0,P.jsx)(`button`,{type:`button`,className:`payment-drawer__backdrop`,"aria-label":`Close payment details`,onClick:r}),(0,P.jsxs)(`aside`,{className:`payment-drawer__panel`,children:[(0,P.jsxs)(`header`,{className:`payment-drawer__header`,children:[(0,P.jsxs)(`div`,{className:`payment-drawer__title-group`,children:[(0,P.jsx)(`p`,{className:`payment-drawer__eyebrow`,children:`Receipt`}),(0,P.jsx)(`h2`,{id:`payment-drawer-title`,children:R(s)}),(0,P.jsxs)(`div`,{className:`payment-drawer__header-meta`,children:[(0,P.jsx)(Oe,{status:p}),(0,P.jsx)(`span`,{children:O(s.paymentDate)})]})]}),(0,P.jsx)(`button`,{type:`button`,className:`button button-secondary payment-drawer__close`,onClick:r,"aria-label":`Close payment details`,children:(0,P.jsx)(A,{size:17})})]}),(0,P.jsxs)(`section`,{className:`payment-drawer__summary-card`,"aria-label":`Payment summary`,children:[(0,P.jsxs)(`div`,{className:`payment-drawer__summary-main`,children:[(0,P.jsxs)(`div`,{children:[(0,P.jsxs)(`h3`,{children:[(0,P.jsx)(`span`,{children:R(s)}),(0,P.jsx)(`button`,{type:`button`,className:`payment-copy-button ${l===`payment-number`?`is-copied`:``}`.trim(),onClick:()=>E(R(s),`payment-number`),"aria-label":`Copy payment number`,title:l===`payment-number`?`Copied`:`Copy payment number`,children:(0,P.jsx)(u,{size:13})})]}),(0,P.jsxs)(`div`,{className:`payment-drawer__summary-meta`,children:[(0,P.jsx)(Oe,{status:p}),(0,P.jsx)(`span`,{children:O(s.paymentDate)})]})]}),(0,P.jsx)(`strong`,{children:m(s.amount)})]}),(0,P.jsxs)(`div`,{className:`payment-drawer__summary-customer`,children:[(0,P.jsx)(`span`,{children:`Customer`}),(0,P.jsx)(`strong`,{children:s.partyName||`Customer`})]})]}),(0,P.jsxs)(`div`,{className:`payment-drawer__content`,children:[(0,P.jsxs)(`section`,{className:`payment-drawer__finance-card`,"aria-label":`Financial summary`,children:[(0,P.jsxs)(`div`,{className:`payment-drawer__section-header`,children:[(0,P.jsx)(`h3`,{children:`Financial Summary`}),(0,P.jsx)(Me,{status:g})]}),(0,P.jsxs)(`div`,{className:`payment-drawer__progress`,children:[(0,P.jsx)(`div`,{className:`payment-drawer__progress-copy`,children:(0,P.jsx)(`span`,{children:`Payment Progress`})}),(0,P.jsx)(`div`,{className:`payment-drawer__progress-track payment-drawer__progress-track--${y}`,"aria-label":`Payment progress ${b}%`,children:(0,P.jsx)(`span`,{style:{width:`${b}%`}})}),(0,P.jsxs)(`div`,{className:`payment-drawer__progress-values`,children:[(0,P.jsxs)(`span`,{children:[`Paid `,(0,P.jsx)(`strong`,{children:m(ee)})]}),(0,P.jsxs)(`span`,{children:[`Remaining `,(0,P.jsx)(`strong`,{children:m(_.outstandingAfter)})]})]})]}),(0,P.jsx)(`div`,{className:`payment-receipt__summary`,"aria-label":`Payment allocation summary`,children:ne.map(e=>(0,P.jsxs)(`div`,{className:[`payment-receipt__summary-row`,e.emphasis?`payment-receipt__summary-row--emphasis`:``,e.strong?`payment-receipt__summary-row--strong`:``,e.tone?`payment-receipt__summary-row--${e.tone}`:``].filter(Boolean).join(` `),children:[(0,P.jsx)(`span`,{children:e.label}),(0,P.jsx)(`strong`,{children:e.value})]},e.label))})]}),(0,P.jsxs)(`section`,{className:`payment-drawer__section payment-drawer__section--compact`,children:[(0,P.jsxs)(`div`,{className:`payment-drawer__section-header`,children:[(0,P.jsx)(`h3`,{children:`Payment Information`}),(0,P.jsxs)(`span`,{className:`payment-drawer__invoice-actions`,children:[(0,P.jsx)(`button`,{type:`button`,className:`payment-drawer__invoice-link`,onClick:()=>o?.(s,t),children:w}),(0,P.jsx)(`button`,{type:`button`,className:`payment-copy-button ${l===`invoice-number`?`is-copied`:``}`.trim(),onClick:()=>E(w,`invoice-number`),"aria-label":`Copy invoice number`,title:l===`invoice-number`?`Copied`:`Copy invoice number`,children:(0,P.jsx)(u,{size:13})})]})]}),(0,P.jsxs)(`dl`,{className:`payment-info-inline`,children:[(0,P.jsx)($,{label:`Method`,value:s.paymentMethod||`Not Provided`}),(0,P.jsx)($,{label:`Reference Number`,value:T?(0,P.jsxs)(`span`,{className:`payment-detail-copy-value`,children:[(0,P.jsx)(`span`,{children:T}),(0,P.jsx)(`button`,{type:`button`,className:`payment-copy-button ${l===`reference-number`?`is-copied`:``}`.trim(),onClick:()=>E(T,`reference-number`),"aria-label":`Copy reference number`,title:l===`reference-number`?`Copied`:`Copy reference number`,children:(0,P.jsx)(u,{size:13})})]}):`Not Provided`}),(0,P.jsx)($,{label:`Notes`,value:s.notes||`No Notes Available`,wide:!0})]})]}),(0,P.jsxs)(`section`,{className:`payment-drawer__section payment-drawer__timeline-card`,children:[(0,P.jsxs)(`div`,{className:`payment-drawer__timeline-header`,children:[(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`h3`,{children:`Payment History`}),(0,P.jsxs)(`span`,{children:[v.length,` Payment`,v.length===1?``:`s`,` Recorded`]})]}),(0,P.jsxs)(`strong`,{children:[`Total Collected: `,m(te)]})]}),v.length>0?(0,P.jsx)(`ol`,{className:`payment-drawer__timeline-list`,children:v.map(e=>{let t=z(e),n=t===C;return(0,P.jsxs)(`li`,{className:n?`is-active`:``,children:[(0,P.jsx)(`span`,{"aria-hidden":`true`}),(0,P.jsxs)(`button`,{type:`button`,onClick:()=>c(e),"aria-current":n?`true`:void 0,children:[(0,P.jsx)(`strong`,{children:R(e)}),(0,P.jsx)(`span`,{className:`payment-drawer__timeline-amount`,children:m(e.amount)}),(0,P.jsx)(`time`,{children:O(e.paymentDate||e.createdAt)})]})]},t)})}):(0,P.jsx)(`div`,{className:`payment-drawer__empty-state`,children:`No Previous Payments`})]})]}),(0,P.jsxs)(`footer`,{className:`payment-drawer__footer`,children:[(0,P.jsxs)(`p`,{className:`payment-drawer__recorded-line`,children:[`Recorded on `,O(s.createdAt||s.paymentDate),` by `,s.createdBy||`System`]}),(0,P.jsxs)(`div`,{className:`payment-drawer__actions`,children:[(0,P.jsxs)(`button`,{type:`button`,className:`button button-secondary`,onClick:()=>a(s),children:[(0,P.jsx)(S,{size:16}),`Download PDF`]}),(0,P.jsxs)(`button`,{type:`button`,className:`button button-primary`,onClick:()=>i([s]),children:[(0,P.jsx)(f,{size:16}),`Print Receipt`]})]})]})]})]})}function tt({payment:e,invoice:t,onSubmit:n,onClose:r,isSubmitting:i}){let[a,o]=(0,N.useState)({amount:String(e.amount||``),paymentMethod:e.paymentMethod||`Bank Transfer`,referenceNumber:e.referenceNumber||``,notes:e.notes||``}),[s,c]=(0,N.useState)({}),l=Ye(e,t).outstandingAfter+Number(e.amount||0),u={amount:_(a.amount,`Amount`,{allowZero:!1})||(Number(a.amount)>l?`Amount cannot exceed ${m(l)}.`:``),referenceNumber:a.referenceNumber.length>100?`Reference number must be 100 characters or fewer.`:``,notes:a.notes.length>500?`Notes must be 500 characters or fewer.`:``},d=Object.values(u).every(e=>!e);function f(e){let{name:t,value:n}=e.target;o(e=>({...e,[t]:n}))}function p(e){c(t=>({...t,[e.target.name]:!0}))}function h(t){t.preventDefault(),c({amount:!0,referenceNumber:!0,notes:!0}),!(!d||i)&&n(e,a)}return(0,P.jsx)(oe,{title:`Edit Payment`,subtitle:`Update receipt values without changing the customer, invoice, or payment number.`,onClose:r,children:(0,P.jsxs)(`form`,{className:`payment-form payment-edit-form`,onSubmit:h,children:[(0,P.jsxs)(`div`,{className:`payment-form__readonly-grid`,children:[(0,P.jsx)($,{label:`Payment Number`,value:R(e)}),(0,P.jsx)($,{label:`Customer`,value:e.partyName}),(0,P.jsx)($,{label:`Invoice Number`,value:q(e,t)})]}),(0,P.jsxs)(`div`,{className:`form-grid`,children:[(0,P.jsx)(ie,{id:`payment-edit-amount`,name:`amount`,label:`Amount`,value:a.amount,onChange:f,onBlur:p,error:s.amount?u.amount:``}),(0,P.jsx)(j,{id:`payment-edit-method`,name:`paymentMethod`,label:`Payment method`,value:a.paymentMethod,onChange:f,options:me}),(0,P.jsx)(ae,{id:`payment-edit-reference`,name:`referenceNumber`,label:`Reference number`,value:a.referenceNumber,maxLength:100,onChange:f,onBlur:p,error:s.referenceNumber?u.referenceNumber:``}),(0,P.jsx)(ae,{id:`payment-edit-notes`,name:`notes`,label:`Notes`,textarea:!0,rows:4,maxLength:500,className:`field--full`,value:a.notes,onChange:f,onBlur:p,error:s.notes?u.notes:``})]}),(0,P.jsxs)(`div`,{className:`button-row payment-form__footer`,children:[(0,P.jsx)(`button`,{type:`submit`,className:`button button-primary`,disabled:!d||i,children:i?`Saving...`:`Save Changes`}),(0,P.jsx)(`button`,{type:`button`,className:`button`,onClick:r,disabled:i,children:`Cancel`})]})]})})}function nt({type:e,partyLabel:t,parties:n,invoices:r,purchaseOrders:i,existingPayments:a,onSubmit:o,onCancel:s,isSubmitting:c}){let l=e===`supplier`,[u,d]=(0,N.useState)({partyId:``,invoiceId:``,poId:``,amount:``,paymentDate:b(),paymentMethod:`Bank Transfer`,referenceNumber:``,notes:``}),[f,p]=(0,N.useState)({}),h=(0,N.useMemo)(()=>r.find(e=>Je(e)===String(u.invoiceId)),[u.invoiceId,r]),g=Number(h?.totalAmount||0),v=Number(h?.paidAmount||0),x=Number(h?.balanceAmount||0),ee=Number(u.amount||0),te=Math.max(0,x-ee),S=u.referenceNumber.trim().toLowerCase(),C=S&&a.some(e=>e.referenceNumber.trim().toLowerCase()===S),w={partyId:y(u.partyId,t),invoiceId:l?``:y(u.invoiceId,`Invoice`),poId:l?y(u.poId,`Purchase order`):``,amount:_(u.amount,`Amount`,{allowZero:!1})||(!l&&h&&Number(u.amount)>x?`Amount cannot exceed outstanding balance.`:``),paymentDate:y(u.paymentDate,`Payment date`),referenceNumber:u.referenceNumber.length>100?`Reference number must be 100 characters or fewer.`:C?`This reference number is already used.`:``,notes:u.notes.length>500?`Notes must be 500 characters or fewer.`:``},T=Object.values(w).every(e=>!e),ne=(0,N.useMemo)(()=>i.map(e=>({value:e.poId||e.id,label:`${e.poNumber||`PO ${e.poId}`} - ${e.supplier||`Supplier`} - ${m(e.totalAmount)}`})),[i]),E=(0,N.useMemo)(()=>r.filter(e=>u.partyId?String(e.customerId)===String(u.partyId):!0).map(e=>({value:Number(e.invoiceId||e.id),label:`${e.invoiceNumber||`Invoice ${e.invoiceId||e.id}`} - ${e.customerName||e.customer||`Customer`} - ${m(e.balanceAmount)}`})).filter(e=>Number.isInteger(e.value)&&e.value>0),[u.partyId,r]);function D(e){let{name:t,value:n}=e.target;d(e=>({...e,[t]:n}))}function O(e){let{value:t}=e.target;d(e=>({...e,partyId:t,invoiceId:!l&&e.invoiceId&&!r.some(n=>String(n.customerId)===String(t)&&Je(n)===String(e.invoiceId))?``:e.invoiceId}))}function k(e){p(t=>({...t,[e.target.name]:!0}))}function A(e){e.preventDefault(),p({partyId:!0,invoiceId:!0,poId:!0,amount:!0,paymentDate:!0,referenceNumber:!0,notes:!0}),!(!T||c)&&o(u)}return(0,P.jsxs)(`form`,{className:`payment-form`,onSubmit:A,autoComplete:`off`,children:[(0,P.jsxs)(`div`,{className:`payment-form__section`,children:[(0,P.jsx)(`div`,{className:`payment-form__section-header`}),(0,P.jsxs)(`div`,{className:`form-grid`,children:[(0,P.jsx)(j,{id:`payment-party`,name:`partyId`,label:t,value:u.partyId,onChange:O,onBlur:k,options:n,placeholder:`Select ${t.toLowerCase()}`,error:w.partyId,showError:f.partyId}),l?(0,P.jsx)(j,{id:`payment-po`,name:`poId`,label:`Purchase order`,value:u.poId,onChange:D,onBlur:k,options:ne,placeholder:`Select purchase order`,error:w.poId,showError:f.poId}):(0,P.jsxs)(`div`,{className:`payment-form__invoice-field`,children:[(0,P.jsx)(j,{id:`payment-invoice`,name:`invoiceId`,label:`Invoice`,value:u.invoiceId,onChange:D,onBlur:k,options:E,placeholder:`Select invoice`,searchPlaceholder:`Search invoices...`,error:w.invoiceId,showError:f.invoiceId}),h?(0,P.jsxs)(`dl`,{className:`payment-form__invoice-metrics`,children:[(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Invoice Total`}),(0,P.jsx)(`dd`,{children:m(g)})]}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Already Paid`}),(0,P.jsx)(`dd`,{children:m(v)})]}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Outstanding Balance`}),(0,P.jsx)(`dd`,{children:m(x)})]})]}):null]})]})]}),(0,P.jsxs)(`div`,{className:`payment-form__section`,children:[(0,P.jsx)(`div`,{className:`payment-form__section-header`,children:(0,P.jsx)(`h3`,{children:`Payment details`})}),(0,P.jsxs)(`div`,{className:`form-grid`,children:[(0,P.jsx)(ie,{id:`payment-amount`,name:`amount`,label:`Amount`,value:u.amount,onChange:D,onBlur:k,error:f.amount?w.amount:``}),(0,P.jsx)(de,{id:`payment-date`,name:`paymentDate`,label:`Payment date`,icon:null,value:u.paymentDate,onChange:D,onBlur:k,error:f.paymentDate?w.paymentDate:``}),(0,P.jsx)(j,{id:`payment-method`,name:`paymentMethod`,label:`Payment method`,value:u.paymentMethod,onChange:D,onBlur:k,options:me}),(0,P.jsx)(ae,{id:`payment-reference`,name:`referenceNumber`,label:`Reference number`,value:u.referenceNumber,maxLength:100,onChange:D,onBlur:k,error:f.referenceNumber?w.referenceNumber:``}),(0,P.jsx)(ae,{id:`payment-notes`,name:`notes`,label:`Notes`,textarea:!0,rows:3,maxLength:500,className:`field--full`,value:u.notes,onChange:D,onBlur:k,error:f.notes?w.notes:``})]})]}),(0,P.jsxs)(`dl`,{className:`payment-form__summary-panel`,children:[(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Invoice Total`}),(0,P.jsx)(`dd`,{children:m(g)})]}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Already Paid`}),(0,P.jsx)(`dd`,{children:m(v)})]}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Current Payment`}),(0,P.jsx)(`dd`,{children:m(ee)})]}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`dt`,{children:`Balance After Payment`}),(0,P.jsx)(`dd`,{children:m(te)})]})]}),(0,P.jsxs)(`div`,{className:`button-row payment-form__footer`,children:[(0,P.jsx)(`button`,{type:`submit`,className:`button button-primary`,disabled:!T||c,children:c?`Posting...`:`Post Payment`}),(0,P.jsx)(`button`,{type:`button`,className:`button`,onClick:s,disabled:c,children:`Cancel`})]})]})}function rt({customers:e,fetchPayments:t,createPayment:n,updatePayment:o,deletePayment:c}){let l=`Customer Payments`,u=`customerPayments`,g=`Customer`,_=e,{hasPermission:y,user:b}=w(),x=ne(),S=xe,C=we.customer,[A,re]=(0,N.useState)([]),[ie,ae]=(0,N.useState)([]),[j,se]=(0,N.useState)(!0),[le,M]=(0,N.useState)(!1),[ue,de]=(0,N.useState)(``),[fe,pe]=(0,N.useState)(!1),[me,ye]=(0,N.useState)(null),[be,I]=(0,N.useState)(null),[L,Ce]=(0,N.useState)(null),[B,Pe]=(0,N.useState)(`all`),[W,K]=(0,N.useState)([]),[Be,He]=(0,N.useState)(()=>$e(C)??S),[Ue,We]=(0,N.useState)(!1),Ge=(0,N.useRef)(null),Ye=(0,N.useRef)(null),Xe=y(u,`create`),J=!!o&&(y(u,`edit`)||y(u,`create`)),Ze=y(u,`delete`),Y=(0,N.useMemo)(()=>new Map(ie.map(e=>[Je(e),e])),[ie]),X=(0,N.useCallback)(async({updateLoading:e=!0,isMounted:n=()=>!0}={})=>{e&&se(!0),de(``);try{let[e,r]=await Promise.all([t(),ce()]);if(!n())return;if(!e.success){de(qe(e.error||e.message)),re([]);return}re((e.data??[]).map(De)),r.success?ae(r.data??[]):de(qe(r.error||`Unable to load invoices.`))}catch(e){n()&&(re([]),de(qe(e instanceof Error?e.message:`Unable to load payment data right now.`)))}finally{n()&&se(!1)}},[t]);(0,N.useEffect)(()=>{let e=!0;return X({isMounted:()=>e}),()=>{e=!1}},[X]),(0,N.useEffect)(()=>{He($e(C)??S)},[S,C]),(0,N.useEffect)(()=>{try{typeof window<`u`&&window.localStorage.setItem(C,JSON.stringify(Be))}catch{}},[C,Be]),(0,N.useEffect)(()=>{let e=new Set(A.map(z));K(t=>t.filter(t=>e.has(t)))},[A]),(0,N.useEffect)(()=>{function e(e){Ge.current?.contains(e.target)||We(!1)}return document.addEventListener(`pointerdown`,e),()=>document.removeEventListener(`pointerdown`,e)},[]);let Z=(0,N.useMemo)(()=>A.filter(e=>{let t=V(e);return!(B===`success`&&t!==`Completed`||B===`pending`&&t!==`Pending`||B===`failed`&&t!==`Failed`||B===`cancelled`&&t!==`Cancelled`||B===`reversed`&&t!==`Reversed`||B===`month`&&!Ne(e.paymentDate,`month`)||B===`last30`&&!Ne(e.paymentDate,`last30`)||B===`large`&&Number(e.amount||0)<ge)}),[B,A]),Q=(0,N.useMemo)(()=>A.filter(e=>W.includes(z(e))),[A,W]);(0,N.useMemo)(()=>Z.filter(e=>W.includes(z(e))),[Z,W]);let $=(0,N.useMemo)(()=>{let e=Z.reduce((e,t)=>e+Number(t.amount||0),0),t=new Date().toISOString().slice(0,7);return{count:Z.length,totalAmount:e,thisMonth:Z.filter(e=>String(e.paymentDate).startsWith(t)).length,reconciled:Z.filter(e=>V(e)===`Completed`).length,pending:Z.filter(e=>V(e)===`Pending`).length}},[Z]);(0,N.useCallback)(e=>{let t=String(e);K(e=>{let n=e.includes(t)?e.filter(e=>e!==t):[...e,t];return A.find(e=>z(e)===t)?.paymentNumber,n.map(e=>A.find(t=>z(t)===e)?.paymentNumber||e),n})},[A]),(0,N.useCallback)(()=>{let e=Z.map(z);K(t=>e.length>0&&e.every(e=>t.includes(e))?t.filter(t=>!e.includes(t)):Array.from(new Set([...t,...e])))},[Z]);let rt=(0,N.useCallback)((e=Z)=>{let t=e.length>0?e:Z;if(t.length===0){h({type:`warning`,title:l,message:`No payments available to export.`});return}Ve(t,!1,b),h({type:`success`,title:l,message:`${t.length} payment${t.length===1?``:`s`} exported as PDF.`})},[Z,l,b]),it=(0,N.useCallback)(async e=>{try{await Ke({payment:e,invoice:Y.get(String(e.invoiceId)),allPayments:A,generatedBy:b?.email||b?.name||`System`}),h({type:`success`,title:l,message:`Receipt PDF downloaded.`})}catch(e){h({type:`error`,title:l,message:e instanceof Error?e.message:`Unable to generate receipt PDF.`})}},[Y,A,l,b]);function at(e=Q){let t=e.length>0?e:Z.slice(0,1);if(t.length===0){h({type:`warning`,title:l,message:`Select a payment to print.`});return}let n=t.map(e=>{let t=Y.get(String(e.invoiceId)),n=Qe(e,t,A),r=Number(n.outstandingAfter||0),i=je(e.invoiceStatus).label,a=H(e.status).label,o=Le();return`
        <section class="receipt-sheet">
          <div class="receipt-top-banner">
            <div class="receipt-brand-container">
              <div class="receipt-logo-badge">IMS</div>
              <div class="receipt-brand-details">
                <strong class="company-name">${U(F.name||_e)}</strong>
                <span class="company-sub">Inventory &amp; Financial Management System</span>
                ${o.map(e=>`<span class="company-line">${U(e)}</span>`).join(``)}
              </div>
            </div>
            <div class="receipt-header-meta">
              <span class="receipt-document-tag">CUSTOMER PAYMENT RECEIPT</span>
              <div class="receipt-number-badge">
                <span class="meta-label">RECEIPT NO:</span>
                <span class="meta-val">${U(R(e))}</span>
              </div>
              <div class="receipt-date-badge">
                <span class="meta-label">DATE:</span>
                <span class="meta-val">${U(Ie(e.paymentDate))}</span>
              </div>
            </div>
          </div>

          <div class="receipt-watermark-stamp">
            <span class="stamp-icon">✓</span>
            <span class="stamp-text">OFFICIAL PAYMENT RECEIPT — RECORDED</span>
          </div>

          <div class="receipt-section-title">Customer &amp; Invoice Details</div>
          <div class="receipt-info-card">
            <div class="info-cell">
              <span class="info-label">Customer Name</span>
              <span class="info-value info-value--large">${U(e.partyName||`Customer`)}</span>
            </div>
            <div class="info-cell">
              <span class="info-label">Invoice Number</span>
              <span class="info-value">${U(q(e,t))}</span>
            </div>
            <div class="info-cell">
              <span class="info-label">Invoice Date</span>
              <span class="info-value">${U(Ie(t?.invoiceDate||e.invoiceDate||e.paymentDate))}</span>
            </div>
            <div class="info-cell">
              <span class="info-label">Invoice Status</span>
              <span class="badge ${Re(i)}">${U(i)}</span>
            </div>
          </div>

          <div class="receipt-section-title">Financial Settlement Breakdown</div>
          <div class="receipt-table-card">
            <table class="receipt-table">
              <thead>
                <tr>
                  <th>Description</th>
                  <th class="text-right">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Invoice Total Amount</td>
                  <td class="text-right font-mono">${U(G(n.invoiceTotal))}</td>
                </tr>
                <tr>
                  <td>Previous Cumulative Payments</td>
                  <td class="text-right font-mono">${U(G(n.previousPayments))}</td>
                </tr>
                <tr class="row-current-payment">
                  <td><strong>Current Payment Settled</strong></td>
                  <td class="text-right font-mono"><strong>${U(G(e.amount))}</strong></td>
                </tr>
                <tr class="${r>0?`row-balance-open`:`row-balance-paid`}">
                  <td><strong>Remaining Outstanding Balance</strong></td>
                  <td class="text-right font-mono"><strong>${U(G(n.outstandingAfter))}</strong></td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="receipt-section-title">Payment Method &amp; Execution</div>
          <div class="receipt-info-card receipt-info-card--three">
            <div class="info-cell">
              <span class="info-label">Payment Method</span>
              <span class="info-value">${U(e.paymentMethod||`Bank Transfer`)}</span>
            </div>
            <div class="info-cell">
              <span class="info-label">Reference / UTR Number</span>
              <span class="info-value font-mono">${U(e.referenceNumber||`N/A`)}</span>
            </div>
            <div class="info-cell">
              <span class="info-label">Payment Status</span>
              <span class="badge ${ze(e.status)}">${U(a)}</span>
            </div>
          </div>

          <div class="receipt-signature-section">
            <div class="signature-box">
              <div class="signature-line"></div>
              <span>Customer Signature</span>
            </div>
            <div class="signature-box">
              <div class="signature-seal-circle">SEAL</div>
              <div class="signature-line"></div>
              <span>Authorized Signatory</span>
            </div>
          </div>

          <footer class="receipt-footer">
            <p class="footer-notice">This is a computer-generated official payment receipt issued by StockPilot IMS. Valid without physical signature.</p>
            <div class="footer-meta">
              <span><b>Generated On:</b> ${U(Fe())}</span>
              <span><b>Issued By:</b> ${U(b?.email||b?.name||`System Administrator`)}</span>
              <span><b>System:</b> ${U(ve)}</span>
            </div>
          </footer>
        </section>
      `}).join(``),r=window.open(``,`_blank`,`width=800,height=850`);if(!r){h({type:`error`,title:l,message:`Unable to open print window.`});return}try{r.opener=null}catch{}let i=!1,a=()=>{i||r.closed||(i=!0,window.setTimeout(()=>{r.closed||(r.focus(),r.print())},150))};r.addEventListener(`load`,a,{once:!0}),r.document.open(),r.document.write(`<!doctype html><html><head><title>Customer Payment Receipt - IMS</title><style>
      @page { size: A4 portrait; margin: 12mm 14mm; }
      * { box-sizing: border-box; -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }
      body { margin: 0; padding: 0; background: #f8fafc; color: #0f172a; font-family: Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; font-size: 13px; line-height: 1.5; }
      .receipt-sheet { background: #ffffff; max-width: 800px; margin: 0 auto; padding: 28px 32px; border: 1px solid #e2e8f0; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.06); position: relative; page-break-after: always; }
      .receipt-sheet:last-child { page-break-after: auto; }
      
      .receipt-top-banner { display: flex; justify-content: space-between; align-items: flex-start; gap: 20px; padding: 20px 24px; background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #2563eb 100%); border-radius: 12px; color: #ffffff; margin-bottom: 20px; }
      .receipt-brand-container { display: flex; align-items: center; gap: 14px; }
      .receipt-logo-badge { width: 50px; height: 50px; border-radius: 12px; background: rgba(255,255,255,0.15); backdrop-filter: blur(4px); border: 1px solid rgba(255,255,255,0.25); display: flex; align-items: center; justify-content: center; font-weight: 900; font-size: 18px; letter-spacing: 1.5px; color: #ffffff; }
      .receipt-brand-details { display: flex; flex-direction: column; }
      .company-name { font-size: 20px; font-weight: 800; letter-spacing: -0.01em; color: #ffffff; line-height: 1.2; }
      .company-sub { font-size: 11px; color: #94a3b8; font-weight: 500; margin-top: 2px; }
      .company-line { font-size: 11px; color: #cbd5e1; margin-top: 1px; }

      .receipt-header-meta { display: flex; flex-direction: column; align-items: flex-end; gap: 6px; text-align: right; }
      .receipt-document-tag { font-size: 11px; font-weight: 800; letter-spacing: 0.1em; color: #60a5fa; text-transform: uppercase; background: rgba(96,165,250,0.15); padding: 4px 10px; border-radius: 999px; border: 1px solid rgba(96,165,250,0.3); }
      .receipt-number-badge, .receipt-date-badge { display: flex; align-items: center; gap: 8px; font-size: 12px; }
      .meta-label { color: #94a3b8; font-size: 10px; font-weight: 700; letter-spacing: 0.05em; }
      .meta-val { color: #ffffff; font-weight: 800; font-family: "JetBrains Mono", Consolas, monospace; }

      .receipt-watermark-stamp { display: flex; align-items: center; justify-content: center; gap: 8px; background: #f0fdf4; border: 1px solid #bbf7d0; padding: 8px 16px; border-radius: 8px; color: #166534; font-size: 12px; font-weight: 800; letter-spacing: 0.04em; margin-bottom: 20px; }
      .stamp-icon { width: 20px; height: 20px; background: #16a34a; color: #ffffff; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 12px; font-weight: 900; }
      .stamp-text { text-transform: uppercase; letter-spacing: 0.06em; }

      .receipt-section-title { font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 0.08em; color: #475569; margin: 18px 0 8px 2px; }
      
      .receipt-info-card { display: grid; grid-template-columns: repeat(4, minmax(0, 1fr)); gap: 12px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 14px 16px; margin-bottom: 16px; }
      .receipt-info-card--three { grid-template-columns: repeat(3, minmax(0, 1fr)); }
      .info-cell { display: flex; flex-direction: column; gap: 4px; }
      .info-label { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.06em; color: #64748b; }
      .info-value { font-size: 13px; font-weight: 700; color: #0f172a; overflow-wrap: anywhere; }
      .info-value--large { font-size: 14px; font-weight: 800; color: #0f172a; }

      .badge { display: inline-flex; align-items: center; align-self: flex-start; padding: 3px 10px; border-radius: 999px; font-size: 11px; font-weight: 800; }
      .badge-green { color: #15803d; background: #dcfce7; border: 1px solid #86efac; }
      .badge-amber { color: #c2410c; background: #fff7ed; border: 1px solid #fed7aa; }
      .badge-red { color: #b91c1c; background: #fef2f2; border: 1px solid #fca5a5; }

      .receipt-table-card { border: 1px solid #e2e8f0; border-radius: 10px; overflow: hidden; margin-bottom: 16px; }
      .receipt-table { width: 100%; border-collapse: collapse; text-align: left; }
      .receipt-table th { background: #f1f5f9; padding: 10px 16px; font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 0.06em; color: #475569; border-bottom: 1px solid #e2e8f0; }
      .receipt-table td { padding: 12px 16px; border-bottom: 1px solid #f1f5f9; font-size: 13px; color: #334155; }
      .receipt-table tr:last-child td { border-bottom: none; }
      .text-right { text-align: right !important; }
      .font-mono { font-family: "JetBrains Mono", Consolas, monospace; }

      .row-current-payment { background: #f0fdf4 !important; border-left: 4px solid #10b981; }
      .row-current-payment td { color: #15803d !important; font-size: 14px; }
      .row-balance-open { background: #fff7ed !important; border-left: 4px solid #f97316; }
      .row-balance-open td { color: #c2410c !important; font-size: 14px; }
      .row-balance-paid { background: #f0fdf4 !important; border-left: 4px solid #10b981; }
      .row-balance-paid td { color: #15803d !important; font-size: 14px; }

      .receipt-signature-section { display: flex; justify-content: space-between; gap: 40px; margin-top: 36px; padding-top: 10px; }
      .signature-box { flex: 1; display: flex; flex-direction: column; align-items: center; text-align: center; position: relative; }
      .signature-seal-circle { width: 44px; height: 44px; border: 2px dashed #cbd5e1; border-radius: 50%; color: #94a3b8; font-size: 9px; font-weight: 800; display: flex; align-items: center; justify-content: center; margin-bottom: -18px; background: #ffffff; z-index: 1; }
      .signature-line { width: 100%; border-bottom: 1.5px dashed #cbd5e1; height: 32px; margin-bottom: 8px; }
      .signature-box span { font-size: 11px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.05em; }

      .receipt-footer { margin-top: 28px; padding-top: 14px; border-top: 1px solid #e2e8f0; text-align: center; color: #64748b; }
      .footer-notice { font-size: 11px; color: #64748b; margin: 0 0 10px; font-weight: 500; }
      .footer-meta { display: flex; justify-content: space-between; gap: 12px; font-size: 11px; color: #94a3b8; background: #f8fafc; padding: 8px 14px; border-radius: 6px; }

      @media print {
        body { background: #ffffff; }
        .receipt-sheet { border: none; box-shadow: none; padding: 0; }
      }
    </style></head><body>${n}</body></html>`),r.document.close(),r.document.readyState===`complete`&&a()}async function ot(){if(Q.length!==0){M(!0);try{await Promise.all(Q.map(e=>c(e.id))),await X({updateLoading:!1}),K([]),h({type:`success`,title:l,message:`Selected payments deleted.`})}catch(e){h({type:`error`,title:l,message:e instanceof Error?e.message:`Unable to delete selected payments.`})}finally{M(!1)}}}async function st(e){M(!0);try{let t=await n({customerId:Ee(e.partyId,`Customer`),invoiceId:Ee(e.invoiceId,`Invoice`),amount:Number(e.amount),paymentDate:e.paymentDate,paymentMethod:e.paymentMethod,referenceNumber:e.referenceNumber.trim(),notes:e.notes.trim(),status:`Completed`,createdBy:b?.email||b?.name||`System`});if(!t.success)throw Error(t.error||`Unable to post payment.`);await X(),h({type:`success`,title:l,message:`Payment posted successfully.`}),pe(!1)}catch(e){h({type:`error`,title:l,message:e instanceof Error?e.message:`Unable to post payment.`})}finally{M(!1)}}async function ct(e,t){if(o){M(!0);try{let n=await o(e.id,{amount:Number(t.amount),paymentMethod:t.paymentMethod,referenceNumber:t.referenceNumber.trim(),notes:t.notes.trim(),status:`Completed`});if(!n.success)throw Error(n.error||`Unable to update payment.`);await X(),I(null),h({type:`success`,title:l,message:`Payment updated successfully.`})}catch(e){h({type:`error`,title:l,message:e instanceof Error?e.message:`Unable to update payment.`})}finally{M(!1)}}}async function lt(){if(!L)return;let e=A;re(e=>e.map(e=>String(e.id)===String(L.id)?{...e,status:`Cancelled`}:e)),Ce(null);let t=await c(L.id);if(!t.success){re(e),h({type:`error`,title:l,message:t.error||`Unable to delete payment.`});return}await X({updateLoading:!1}),h({type:`success`,title:l,message:`Payment deleted successfully.`})}function ut(e){Se.includes(e)||He(t=>{let n=xe,r=t.length>0?t:n,i=r.includes(e)?r.filter(t=>t!==e):[...r,e];return Se.reduce((e,t)=>e.includes(t)?e:[...e,t],i)})}function dt(){He(xe)}let ft=(0,N.useMemo)(()=>[{key:`paymentNumber`,label:`Payment No`,className:`payments-col-number`,mobilePrimary:!0,sortable:!0,sortValue:e=>R(e),render:e=>(0,P.jsx)(`strong`,{className:`payments-page__record-number payments-readable-cell`,title:R(e),children:R(e)}),searchValue:e=>`${R(e)} ${e.partyName} ${e.invoiceNumber} ${e.invoiceStatus} ${e.referenceNumber} ${e.status}`},{key:`paymentDate`,label:`Payment Date`,className:`payments-col-date`,sortable:!0,sortValue:e=>e.paymentDate,render:e=>{let t=O(e.paymentDate);return(0,P.jsx)(`span`,{className:`payments-readable-cell`,title:t,children:t})}},{key:`partyName`,label:g,className:`payments-col-party`,sortable:!0,render:e=>{if(e.partyName&&e.partyName!==`-`)return e.partyName;let t=e.customerId||e.supplierId||e.partyId;if(t&&Array.isArray(_)){let e=_.find(e=>String(e.id)===String(t)||String(e.customerId)===String(t)||String(e.supplierId)===String(t));if(e)return e.name||e.companyName||e.company||e.customerName||e.supplierName||`-`}return e.partyName||`-`}},{key:`invoiceNumber`,label:`Invoice No`,className:`payments-col-document`,sortable:!0,sortValue:e=>q(e,Y.get(String(e.invoiceId))),render:e=>{let t=q(e,Y.get(String(e.invoiceId)));return(0,P.jsx)(`span`,{className:`payments-page__document-number payments-readable-cell`,title:t,children:t})}},{key:`invoiceStatus`,label:`Invoice Status`,className:`payments-col-invoice-status`,mobileStatus:!0,sortable:!0,sortValue:e=>Ae(e.invoiceStatus),render:e=>(0,P.jsx)(Me,{status:e.invoiceStatus})},{key:`amount`,label:`Amount`,className:`is-numeric payments-col-amount`,sortable:!0,sortValue:e=>Number(e.amount||0),render:e=>m(e.amount)},{key:`paymentMethod`,label:`Method`,className:`payments-col-method`,sortable:!0},{key:`referenceNumber`,label:`Reference Number`,className:`payments-col-reference`,sortable:!0,render:e=>e.referenceNumber||`Not provided`},{key:`status`,label:`Status`,className:`payments-col-status`,mobileStatus:!0,sortable:!0,sortValue:e=>V(e),render:e=>(0,P.jsx)(`div`,{className:`payments-status-menu`,"data-row-click-ignore":`true`,children:(0,P.jsx)(Oe,{status:V(e)})})},{key:`createdBy`,label:`Created By`,className:`payments-col-created-by`,sortable:!0,render:e=>e.createdBy||`System`},{key:`notes`,label:`Notes`,className:`payments-col-notes`,sortable:!0,render:e=>e.notes||`No notes`},{key:`cancelledAt`,label:`Cancelled At`,className:`payments-col-cancelled-at`,sortable:!0,render:e=>e.cancelledAt?O(e.cancelledAt):`Not cancelled`},{key:`cancellationReason`,label:`Cancellation Reason`,className:`payments-col-cancellation-reason`,sortable:!0,render:e=>e.cancellationReason||`Not provided`},{key:`actions`,label:`Actions`,className:`payments-col-actions`,searchable:!1,render:e=>(0,P.jsx)(r,{iconOnly:!0,label:`Actions for ${R(e)}`,actions:[{key:`view`,label:`View`,icon:te,onClick:()=>ye(e)},J?{key:`edit`,label:`Edit`,icon:d,onClick:()=>I(e)}:null,{key:`download`,label:`Download receipt`,icon:s,onClick:()=>it(e)}]})}],[J,Z,it,Y,!1,g]),pt=(0,N.useMemo)(()=>{let e=new Set(ft.map(e=>e.key)),t=xe,n=Be.filter(t=>e.has(t)),r=n.length>0?n:t.filter(t=>e.has(t));return Se.forEach(t=>{e.has(t)&&!r.includes(t)&&r.push(t)}),ft.filter(e=>r.includes(e.key))},[ft,!1,Be]),mt=xe,ht=(0,N.useMemo)(()=>new Set(mt),[mt]),gt=pt.some(e=>!ht.has(e.key)),_t=pt.map(e=>e.key).join(`|`),vt=pt.length,yt=(0,N.useMemo)(()=>pt.map(e=>{let t=Te[e.key];if(!t)return e;let n={width:`${t}px`,minWidth:`${t}px`,maxWidth:`${t}px`};return{...e,tableWidth:t,style:{...e.style||{},...n},headerStyle:{...e.headerStyle||e.style||{},...n}}}),[!1,pt]),bt=[`card`,`payments-page__table-card`,`payments-page__table-card--customer`,gt?`payments-page__table-card--expanded-columns`:``,`payments-page__table-card--overflow-columns`,`payments-page__table-card--${vt}-columns`].filter(Boolean).join(` `),xt=ft.filter(e=>!Se.includes(e.key)&&typeof e.label==`string`),St=W.length>0?(0,P.jsxs)(i,{className:`payments-page__selected-actions`,ariaLabel:`Selected payment actions`,children:[(0,P.jsxs)(`div`,{className:`payments-selection-summary`,"aria-live":`polite`,"data-selection-mode":`true`,children:[(0,P.jsx)(E,{size:15}),(0,P.jsxs)(`strong`,{children:[W.length,` selected`]})]}),(0,P.jsxs)(`button`,{type:`button`,className:`button button-secondary payments-toolbar-button`,onClick:()=>rt(Q),children:[(0,P.jsx)(s,{size:15}),`Export`]}),(0,P.jsxs)(`button`,{type:`button`,className:`button button-secondary payments-toolbar-button`,onClick:()=>at(Q),children:[(0,P.jsx)(f,{size:15}),`Print`]}),Ze?(0,P.jsxs)(`button`,{type:`button`,className:`button button-secondary payments-toolbar-button payments-toolbar-button--danger`,onClick:ot,disabled:le,children:[(0,P.jsx)(p,{size:15}),`Delete`]}):null]}):null,Ct=(0,P.jsxs)(i,{className:`payments-page__toolbar-actions`,ariaLabel:`${l} table actions`,children:[(0,P.jsx)(`label`,{className:`payments-toolbar-select`,children:(0,P.jsx)(`select`,{value:B,onChange:e=>Pe(e.target.value),children:he.map(e=>(0,P.jsx)(`option`,{value:e.value,children:e.label},e.value))})}),W.length===0?(0,P.jsxs)(P.Fragment,{children:[(0,P.jsxs)(`div`,{className:`payments-column-filter`,ref:Ge,children:[(0,P.jsxs)(`button`,{ref:Ye,type:`button`,className:`button button-secondary payments-column-filter__trigger`,"aria-haspopup":`menu`,"aria-expanded":Ue,onClick:()=>We(e=>!e),children:[(0,P.jsx)(v,{size:15}),`Columns`]}),Ue?(0,P.jsxs)(k,{anchorRef:Ye,className:`payments-column-filter__menu payments-column-filter__menu--portal`,width:230,children:[(0,P.jsxs)(`div`,{className:`payments-column-filter__menu-header`,children:[(0,P.jsx)(`strong`,{children:`Visible columns`}),(0,P.jsx)(`button`,{type:`button`,onClick:dt,children:`Reset`})]}),(0,P.jsx)(`div`,{className:`payments-column-filter__options`,children:xt.map(e=>{let t=pt.some(t=>t.key===e.key);return(0,P.jsxs)(`label`,{className:`payments-column-filter__option`,children:[(0,P.jsx)(`input`,{type:`checkbox`,checked:t,onChange:()=>ut(e.key)}),(0,P.jsx)(`span`,{className:`payments-column-filter__check`,"aria-hidden":`true`,children:t?(0,P.jsx)(E,{size:13}):null}),(0,P.jsx)(`span`,{children:e.label})]},e.key)})})]}):null]}),(0,P.jsxs)(`button`,{type:`button`,className:`button button-secondary payments-toolbar-button`,onClick:()=>rt(Z),disabled:Z.length===0,children:[(0,P.jsx)(s,{size:15}),`Export`]})]}):null]}),wt=!!ue&&!j;return $.count,$.reconciled,$.pending,ke($.totalAmount),(0,P.jsxs)(`div`,{className:`page payments-page payments-page--customer`,children:[(0,P.jsxs)(`header`,{className:`resource-center__inventory-header`,"aria-label":`Customer Payments summary`,children:[(0,P.jsxs)(`div`,{className:`resource-center__inventory-header-main`,children:[(0,P.jsx)(`h1`,{children:`Customer Payments`}),(0,P.jsxs)(`div`,{className:`resource-center__inventory-metrics`,children:[(0,P.jsxs)(`span`,{className:`resource-center__inventory-metric resource-center__inventory-metric--success`,children:[(0,P.jsx)(`strong`,{children:$.count}),` Payments`]}),(0,P.jsxs)(`span`,{className:`resource-center__inventory-metric resource-center__inventory-metric--success`,children:[(0,P.jsx)(`strong`,{children:$.reconciled}),` Success`]}),(0,P.jsxs)(`span`,{className:`resource-center__inventory-metric resource-center__inventory-metric--warning`,children:[(0,P.jsx)(`strong`,{children:$.pending}),` Pending`]}),(0,P.jsxs)(`span`,{className:`resource-center__inventory-metric resource-center__inventory-metric--info`,children:[(0,P.jsx)(`strong`,{children:ke($.totalAmount)}),` Collected`]})]})]}),(0,P.jsx)(`div`,{className:`resource-center__inventory-header-actions`,children:Xe?(0,P.jsxs)(`button`,{type:`button`,className:`button button-primary`,onClick:()=>pe(!0),children:[(0,P.jsx)(ee,{size:16}),`New Payment`]}):null})]}),wt?(0,P.jsx)(`div`,{className:`card payments-page__state-card`,children:(0,P.jsx)(T,{type:`server`,title:`Unable to load payment data`,message:`${ue} Please try again in a few moments.`,actionLabel:`Retry`,onAction:()=>X(),compact:!0})}):(0,P.jsx)(`div`,{className:`${bt} resource-center__inventory-table-card`,children:(0,P.jsx)(a,{className:`resource-center__inventory-table`,rows:Z,columns:yt,keyField:`paymentRowId`,loading:j,defaultPageSize:20,allowSortReset:!0,showSearch:W.length===0,searchKeys:[`paymentNumber`,`partyName`,`invoiceNumber`,`invoiceStatus`,`referenceNumber`],searchPlaceholder:`Search payments`,emptyMessage:`No customer payments available.`,hideSelectionSummary:!0,filterContent:St,toolbarContent:Ct,rowClassName:e=>W.includes(z(e))?`is-selected`:``,showColumnControls:!1,enableRowSelection:!0,selectedRowKeys:W,onSelectionChange:K,fitExplicitColumnsToContainer:!gt,showHorizontalScrollbar:gt,splitToolbar:!0},_t)}),fe?(0,P.jsx)(oe,{title:`Post ${g} Payment`,onClose:()=>pe(!1),children:(0,P.jsx)(nt,{type:`customer`,partyLabel:g,parties:_,invoices:ie,purchaseOrders:[],existingPayments:A,onSubmit:st,onCancel:()=>pe(!1),isSubmitting:le})}):null,me?(0,P.jsx)(et,{payment:me,invoice:Y.get(String(me.invoiceId)),allPayments:A,onClose:()=>ye(null),onPrint:at,onDownloadReceipt:it,onOpenInvoice:e=>{let t=e?.invoiceId;x(t?`/management/accounting/${t}`:`/management/accounting`)}}):null,be?(0,P.jsx)(tt,{payment:be,invoice:Y.get(String(be.invoiceId)),onSubmit:ct,onClose:()=>I(null),isSubmitting:le}):null,L?(0,P.jsx)(oe,{title:L.deleteMode===`void`?`Void Payment`:`Delete Payment`,onClose:()=>Ce(null),children:(0,P.jsxs)(`div`,{className:`payment-delete-dialog`,children:[(0,P.jsx)(`div`,{className:`payment-delete-dialog__icon`,children:(0,P.jsx)(D,{size:18})}),(0,P.jsxs)(`div`,{children:[(0,P.jsx)(`h3`,{children:L.deleteMode===`void`?`Void this payment?`:`Delete this payment?`}),(0,P.jsxs)(`p`,{children:[`Payment `,(0,P.jsx)(`strong`,{children:R(L)}),` for `,(0,P.jsx)(`strong`,{children:L.partyName}),` worth`,` `,(0,P.jsx)(`strong`,{children:m(L.amount)}),` will be cancelled and the ledger balances will be reversed.`]})]}),(0,P.jsxs)(`div`,{className:`button-row payment-delete-dialog__actions`,children:[(0,P.jsx)(`button`,{type:`button`,className:`button button-secondary`,onClick:()=>Ce(null),children:`Cancel`}),(0,P.jsx)(`button`,{type:`button`,className:`button button-danger`,onClick:lt,children:L.deleteMode===`void`?`Void Payment`:`Delete Payment`})]})]})}):null]})}function it({customers:e}){let[t,n]=(0,N.useState)(e||[]);return(0,N.useEffect)(()=>{g().then(e=>{e?.success&&Array.isArray(e?.data)&&e.data.length>0&&n(e.data)}).catch(()=>{})},[]),(0,P.jsx)(rt,{customers:t?.length?t:e,fetchPayments:M,createPayment:ue,updatePayment:se,deletePayment:le})}export{it as default};