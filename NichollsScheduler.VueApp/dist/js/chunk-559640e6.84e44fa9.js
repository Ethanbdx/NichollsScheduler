(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-559640e6"],{"1f4f":function(t,e,s){"use strict";s("a9e3");var a=s("5530"),r=(s("8b37"),s("80d2")),i=s("7560"),n=s("58df");e["a"]=Object(n["a"])(i["a"]).extend({name:"v-simple-table",props:{dense:Boolean,fixedHeader:Boolean,height:[Number,String]},computed:{classes:function(){return Object(a["a"])({"v-data-table--dense":this.dense,"v-data-table--fixed-height":!!this.height&&!this.fixedHeader,"v-data-table--fixed-header":this.fixedHeader},this.themeClasses)}},methods:{genWrapper:function(){return this.$slots.wrapper||this.$createElement("div",{staticClass:"v-data-table__wrapper",style:{height:Object(r["d"])(this.height)}},[this.$createElement("table",this.$slots.default)])}},render:function(t){return t("div",{staticClass:"v-data-table",class:this.classes},[this.$slots.top,this.genWrapper(),this.$slots.bottom])}})},"84a6":function(t,e,s){"use strict";s.r(e);var a=function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("v-container",[s("v-row",[s("v-col",{attrs:{cols:"12"}},[s("h1",{attrs:{"text-center":""}},[t._v("Use these CRNs to register on Banner.")])])],1),s("v-row",t._l(t.desiredCourses,(function(e){return s("v-col",{key:e.courseRegistrationNum,staticClass:"d-flex",staticStyle:{"flex-direction":"column"},attrs:{cols:"12",lg:"6",xl:"4"}},[s("v-card",{staticClass:"mb-1 flex-grow-1"},[s("v-card-title",[s("v-container",[s("v-row",[s("v-col",[s("h1",{staticClass:"text-decoration-underline primary--text"},[t._v("CRN: "+t._s(e.courseRegistrationNum))])])],1),s("v-row",{attrs:{"no-gutters":""}},[s("v-col",{attrs:{cols:"12"}},[s("h4",[t._v(t._s(e.searchModel.subjectCode)+" "+t._s(e.searchModel.courseNumber)+" - "+t._s(e.section))]),null!=e.topic?[t._v(" - "+t._s(e.topic))]:t._e()],2)],1),s("v-row",{attrs:{"no-gutters":""}},[s("v-col",[s("h4",[t._v(t._s(e.searchModel.courseTitle))])])],1)],1)],1),s("v-card-subtitle",[s("v-chip",{attrs:{pill:"",light:""}},[s("v-icon",{attrs:{left:""}},[t._v("mdi-briefcase-account")]),s("h5",[t._v(t._s(e.instructor[0]))])],1)],1),s("v-card-text",[s("div",[s("h4",[t._v("Remaining Seats: "+t._s(e.remainingSeats))]),s("h4",[t._v("Waitlist Remaining: "+t._s(e.remainingWaitlist))])]),s("div",[s("v-simple-table",{attrs:{dense:"",light:""}},[s("thead",[s("tr",[s("th",[t._v("Days")]),s("th",[t._v("Times")]),s("th",[t._v("Location")])])]),s("tbody",t._l(e.days.length,(function(a){return s("tr",{key:e.courseRegistrationNum+e.days[a-1]},[s("td",[t._v(t._s(e.days[a-1]))]),s("td",[t._v(t._s(e.time[a-1]))]),s("td",[t._v(t._s(e.location[a-1]))])])})),0)])],1)])],1)],1)})),1),s("v-divider"),s("v-row",[s("v-col",{attrs:{cols:"12",lg:"4","offset-lg":"4"}},[s("v-btn",{attrs:{block:"","x-large":"",color:"primary",link:"",href:"https://banner.nicholls.edu/prod/twbkwbis.P_WWWLogin",target:"_blank"}},[t._v("Open Banner in New Tab")])],1)],1)],1)},r=[],i=(s("07ac"),{name:"ConfirmSchedule",computed:{desiredCourses:function(){return Object.values(this.$store.getters.selectedResults)}}}),n=i,o=s("2877"),l=s("6544"),c=s.n(l),d=s("8336"),h=s("b0af"),v=s("99d9"),u=s("cc20"),_=s("62ad"),b=s("a523"),f=s("ce7e"),p=s("132d"),m=s("0fd9"),g=s("1f4f"),w=Object(o["a"])(n,a,r,!1,null,null,null);e["default"]=w.exports;c()(w,{VBtn:d["a"],VCard:h["a"],VCardSubtitle:v["b"],VCardText:v["c"],VCardTitle:v["d"],VChip:u["a"],VCol:_["a"],VContainer:b["a"],VDivider:f["a"],VIcon:p["a"],VRow:m["a"],VSimpleTable:g["a"]})},"8b37":function(t,e,s){}}]);
//# sourceMappingURL=chunk-559640e6.84e44fa9.js.map