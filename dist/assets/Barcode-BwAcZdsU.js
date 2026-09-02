import{r as e}from"./rolldown-runtime-QTnfLwEv.js";import{c as t,d as n,f as r,h as i,r as a,t as o}from"./jsx-runtime-Md2kjDfB.js";import{c as s,s as c}from"./erp-D2tD2v1L.js";import{t as l}from"./calendar-days-BX12kxgl.js";import{t as u}from"./printer-jNtDF2O3.js";import{t as d}from"./save-BJXRX0Ov.js";import{H as f,L as p,Nn as m,Pn as h,R as g,V as _,g as v,h as y}from"./index-BN9BF01H.js";import{t as b}from"./PageHeader-Dt27Q7Yi.js";import{t as x}from"./SearchableSelect-BVllMUO4.js";import{t as S}from"./SelectWithAdd--9f1kSi4.js";var C=e(i(),1);function w(e){return String(e||`BARCODE`).replace(/\s+/g,``).split(``).flatMap((e,t)=>{let n=e.charCodeAt(0);return[{key:`${t}-a`,width:n%3+1,height:54-(n+t)%3*8},{key:`${t}-b`,width:(n+1)%2+1,height:54},{key:`${t}-c`,width:(n+t)%4+1,height:54-(n+t)%2*6}]})}function T(e){return e.split(``).reduce((e,t,n)=>(e*31+t.charCodeAt(0)+n)%2147483647,7)}function E(e,t,n,r){let i=e-n,a=t-r;return i<0||i>6||a<0||a>6?null:i===0||i===6||a===0||a===6||i>=2&&i<=4&&a>=2&&a<=4}function D(e){let t=T(e||`QR`);return Array.from({length:441},(n,r)=>{let i=Math.floor(r/21),a=r%21,o=E(i,a,0,0)??E(i,a,0,14)??E(i,a,14,0);if(o!==null)return{key:`${i}-${a}`,dark:o};let s=e.charCodeAt((i+a)%e.length)||17,c=(t+i*17+a*23+i*a+s)%11;return{key:`${i}-${a}`,dark:c%2==0||c===7}})}function O(e,t){return e?t===`QR Code`?`QR:${e.id}:${e.sku||e.name}`:`${e.sku||e.name}-STATIC`:``}var k=o();function A({value:e}){let t=w(e);return(0,k.jsxs)(`div`,{className:`barcode-visual`,"aria-label":`Barcode preview for ${e}`,children:[(0,k.jsx)(`div`,{className:`barcode-visual__bars`,children:t.map(e=>(0,k.jsx)(`span`,{className:`barcode-visual__bar`,style:{width:`${e.width}px`,height:`${e.height}px`}},e.key))}),(0,k.jsx)(`span`,{className:`barcode-visual__label`,children:e})]})}function j({value:e}){let t=D(e);return(0,k.jsxs)(`div`,{className:`qr-visual`,"aria-label":`QR preview for ${e}`,children:[(0,k.jsx)(`div`,{className:`qr-visual__grid`,children:t.map(e=>(0,k.jsx)(`span`,{className:`qr-visual__cell ${e.dark?`is-dark`:``}`},e.key))}),(0,k.jsx)(`span`,{className:`qr-visual__label`,children:e})]})}function M({codeType:e,value:t}){return e===`QR Code`?(0,k.jsx)(j,{value:t}):(0,k.jsx)(A,{value:t})}function N({formData:e,touched:t,errors:n,products:r,livePreviewValue:i,onChange:a,onBlur:o,onSubmit:s,onCancel:c,onQuickAddProduct:u,isSaving:m=!1}){let h=Object.values(n).every(e=>!e);return(0,k.jsxs)(`div`,{className:`card`,children:[(0,k.jsx)(`h2`,{className:`section-title`,children:`Generate Barcode / QR`}),(0,k.jsxs)(`form`,{className:`form-grid`,onSubmit:s,autoComplete:`off`,children:[(0,k.jsx)(S,{id:`barcode-product`,name:`productId`,label:`Product`,icon:f,value:e.productId,onChange:a,onBlur:o,options:r,placeholder:`Select product`,error:n.productId,showError:t.productId,onAddOption:u,addLabel:`+ Add`,addTitle:`Add Product`,addFields:[{name:`name`,label:`Product Name`,placeholder:`Enter product name`},{name:`sku`,label:`SKU`,placeholder:`Enter SKU`,required:!1}]}),(0,k.jsx)(x,{id:`barcode-type`,name:`codeType`,label:`Code Type`,icon:p,value:e.codeType,onChange:a,options:[`Barcode`,`QR Code`],placeholder:`Select code type`}),(0,k.jsxs)(`div`,{className:`field`,children:[(0,k.jsx)(`label`,{htmlFor:`barcode-date`,children:`Date`}),(0,k.jsxs)(`div`,{className:`input-with-icon`,children:[(0,k.jsx)(l,{size:16}),(0,k.jsx)(`input`,{id:`barcode-date`,name:`date`,type:`date`,value:e.date,onChange:a,onBlur:o})]}),t.date&&n.date?(0,k.jsx)(`span`,{className:`field-error`,children:n.date}):null]}),(0,k.jsxs)(`div`,{className:`barcode-page__note`,children:[(0,k.jsx)(`strong`,{children:`Static preview:`}),(0,k.jsx)(`span`,{children:`This page shows built-in visual barcode and QR previews. We can replace them with a standards-compliant library later.`})]}),i?(0,k.jsxs)(`div`,{className:`barcode-page__live-preview`,children:[(0,k.jsx)(`h3`,{children:`Preview`}),(0,k.jsx)(M,{codeType:e.codeType,value:i})]}):null,(0,k.jsxs)(`div`,{className:`button-row field--full`,children:[(0,k.jsxs)(`button`,{type:`submit`,className:`button button-primary`,disabled:!h||m,children:[(0,k.jsx)(d,{size:16}),m?`Generating...`:`Generate`]}),(0,k.jsxs)(`button`,{type:`button`,className:`button`,onClick:c,children:[(0,k.jsx)(g,{size:16}),`Cancel`]})]})]})]})}function P(e){return String(e??``).replace(/&/g,`&amp;`).replace(/</g,`&lt;`).replace(/>/g,`&gt;`).replace(/"/g,`&quot;`).replace(/'/g,`&#39;`)}function F(e){if(!e)return;let t=e.codeType===`QR Code`,n=window.open(``,`_blank`);if(!n)return;let r=``;r=t?`
      <div style="display: grid; grid-template-columns: repeat(21, 8px); gap: 1px; justify-content: center; background: #ffffff; padding: 12px; border: 1px solid #cbd5e1; border-radius: 8px; margin: 0 auto; width: fit-content;">
        ${D(e.value).map(e=>`<span style="width: 8px; height: 8px; background-color: ${e.dark?`#000000`:`#ffffff`}; display: block;"></span>`).join(``)}
      </div>
    `:`
      <div style="display: flex; align-items: flex-end; justify-content: center; gap: 2px; height: 64px; padding: 12px 16px; background: #ffffff; border: 1px solid #cbd5e1; border-radius: 8px; margin: 0 auto; width: fit-content;">
        ${w(e.value).map(e=>`<span style="width: ${Math.max(2,e.width*2.2)}px; height: ${Math.max(20,e.height*1.15)}px; background-color: #000000; display: inline-block;"></span>`).join(``)}
      </div>
    `,n.document.write(`<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>Print Code - ${P(e.productName||`Barcode`)}</title>
  <style>
    * { box-sizing: border-box; }
    body {
      margin: 0;
      padding: 40px 20px;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
      background: #ffffff;
      color: #0f172a;
      -webkit-print-color-adjust: exact !important;
      print-color-adjust: exact !important;
    }
    .label-card {
      max-width: 360px;
      margin: 0 auto;
      border: 2px solid #0f172a;
      border-radius: 14px;
      padding: 24px;
      text-align: center;
      background: #ffffff;
      box-shadow: 0 4px 12px rgba(0,0,0,0.05);
    }
    .label-header {
      border-bottom: 1.5px solid #e2e8f0;
      padding-bottom: 14px;
      margin-bottom: 18px;
    }
    .product-title {
      font-size: 19px;
      font-weight: 800;
      color: #0f172a;
      margin: 0 0 6px;
      letter-spacing: -0.01em;
    }
    .code-type-tag {
      display: inline-block;
      font-size: 11px;
      font-weight: 750;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      color: #047857;
      background: #ecfdf5;
      padding: 4px 12px;
      border-radius: 9999px;
      border: 1px solid #a7f3d0;
    }
    .visual-wrapper {
      margin: 18px 0;
    }
    .code-value {
      font-family: SFMono-Regular, Consolas, "Liberation Mono", Menlo, monospace;
      font-size: 15px;
      font-weight: 750;
      letter-spacing: 0.12em;
      color: #0f172a;
      margin-top: 12px;
      word-break: break-all;
    }
    .label-footer {
      border-top: 1.5px solid #e2e8f0;
      padding-top: 12px;
      margin-top: 18px;
      font-size: 11px;
      color: #64748b;
      display: flex;
      justify-content: space-between;
      font-weight: 600;
    }
    @media print {
      body { padding: 10mm; }
      .label-card { border-color: #000000; box-shadow: none; }
    }
  </style>
</head>
<body>
  <div class="label-card">
    <div class="label-header">
      <h2 class="product-title">${P(e.productName||`Product`)}</h2>
      <span class="code-type-tag">${P(e.codeType||`Barcode`)}</span>
    </div>
    <div class="visual-wrapper">
      ${r}
      <div class="code-value">${P(e.value||``)}</div>
    </div>
    <div class="label-footer">
      <span>Date: ${P(e.date||``)}</span>
      <span>IMS Inventory</span>
    </div>
  </div>
</body>
</html>`),n.document.close(),n.focus(),n.print()}function I({barcodes:e}){return(0,k.jsx)(`div`,{className:`card barcode-page__table-card`,children:(0,k.jsx)(c,{rows:e,columns:[{key:`date`,label:`Date`,sortable:!0},{key:`productName`,label:`Product`,sortable:!0,mobilePrimary:!0,searchValue:e=>`${e.productName} ${e.codeType} ${e.value}`},{key:`codeType`,label:`Type`,sortable:!0,mobileStatus:!0},{key:`value`,label:`Value`,sortable:!0,className:`barcode-page__code`},{key:`preview`,label:`Preview`,searchable:!1,mobileHidden:!0,render:e=>(0,k.jsx)(M,{codeType:e.codeType,value:e.value}),className:`barcode-page__preview`},{key:`actions`,label:`Actions`,searchable:!1,hideable:!1,tableWidth:80,render:e=>(0,k.jsx)(s,{iconOnly:!0,label:`Actions for ${e.productName||`Barcode`}`,actions:[{key:`print`,label:`Print`,icon:u,onClick:()=>F(e)}]})}],defaultPageSize:8,splitToolbar:!0,searchPlaceholder:`Search codes by product, type, or value...`,emptyMessage:`No barcode or QR records available.`})})}function L(){return t(a.barcode.list)}async function R(e=[]){let t=await L();if(!t.success)return t;let n=r(t).map(t=>B(t,e));return{...t,data:n}}async function z(e,r=[]){let i=await t(a.barcode.generate,{method:`POST`,query:{productId:Number(e)||e}});if(!i.success)return i;let o=B(n(i,{}),r);return{...i,data:o}}function B(e={},t=[]){let n=String(e.id||e.barcodeId||e.BarcodeId||``),r=String(e.productId||e.ProductId||``),i=t.find(e=>String(e.id)===r||String(e.productId)===r),a=e.productName||e.ProductName||i?.name||`Unknown Product`,o=e.value||e.Value||e.code||e.Code||``,s=e.codeType||e.CodeType||(o.startsWith(`QR:`)?`QR Code`:`Barcode`),c=e.date||e.Date||e.createdAt||e.CreatedAt||new Date().toISOString().split(`T`)[0],l=String(c).split(`T`)[0];return{...e,id:n,productId:r,productName:a,codeType:s,value:o,preview:s===`QR Code`?`[ QR ] ${a}`:`|||| ${o} ||||`,date:l}}var V={productId:``,codeType:`Barcode`,date:h()};function H({products:e=[],onQuickAddProduct:t}){let{hasPermission:n}=v(),[r,i]=(0,C.useState)([]),[a,o]=(0,C.useState)(!1),[s,c]=(0,C.useState)(V),[l,u]=(0,C.useState)({}),[d,f]=(0,C.useState)(null),[h,g]=(0,C.useState)(!0),[x,S]=(0,C.useState)(!1),[w,T]=(0,C.useState)(``),E=n(`barcode`,`create`),D={productId:m(s.productId,`Product`),date:m(s.date,`Date`)},A=O(e.find(e=>String(e.id)===String(s.productId))??null,s.codeType);async function j(){g(!0),T(``);try{let t=await R(e);t.success?i(t.data):T(t.error||`Failed to load barcodes.`)}catch{T(`An unexpected error occurred while loading barcodes.`)}finally{g(!1)}}(0,C.useEffect)(()=>{j()},[e]),(0,C.useEffect)(()=>{if(d){let e=setTimeout(()=>{f(null)},4e3);return()=>clearTimeout(e)}},[d]);function M(e){let{name:t,value:n}=e.target;c(e=>({...e,[t]:n}))}function P(e){let{name:t}=e.target;u(e=>({...e,[t]:!0}))}async function F(t){if(t.preventDefault(),u({productId:!0,date:!0}),!Object.values(D).some(Boolean)){S(!0),f(null);try{let t=await z(s.productId,e);t.success?(i(e=>[t.data,...e]),c(V),u({}),o(!1),f({success:!0,message:`${s.codeType} generated successfully.`})):f({success:!1,message:t.error||`Failed to generate code.`})}catch{f({success:!1,message:`An unexpected error occurred while generating code.`})}finally{S(!1)}}}function L(e){let n=t(e);return f(n),n.success?n.item:null}function B(){c(V),u({}),o(!1)}return(0,k.jsxs)(`div`,{className:`page barcode-page`,children:[(0,k.jsx)(b,{icon:p,title:`Barcode / QR`,description:``,actions:E?(0,k.jsxs)(`button`,{type:`button`,className:`button button-primary`,onClick:()=>o(e=>!e),children:[(0,k.jsx)(_,{size:16}),`Add Code`]}):null}),d?(0,k.jsx)(`div`,{className:`message-box ${d.success?`message-box--success`:`message-box--error`}`,children:d.message}):null,a?(0,k.jsx)(N,{formData:s,touched:l,errors:D,products:e,livePreviewValue:A,onChange:M,onBlur:P,onSubmit:F,onCancel:B,onQuickAddProduct:L,isSaving:x}):null,h?(0,k.jsx)(y,{type:`loading`,title:`Loading Barcodes`,message:`Fetching dynamic barcodes from the backend...`}):w?(0,k.jsx)(y,{type:`error`,title:`Failed to Load Barcodes`,message:w,actionLabel:`Retry`,onAction:j}):(0,k.jsx)(I,{barcodes:r})]})}export{H as default};