﻿//<template>
//    <div :id="id">
//        <el-form ref="form" :model="form" label-width="80px">
//              <el-form-item label="活动名称">
//                <el-input v-model="form.name"></el-input>
//          </el-form-item>
//          <el-form-item label="活动区域">
//            <el-select v-model="form.region" placeholder="请选择活动区域">
//              <el-option label="区域一" value="shanghai"></el-option>
//              <el-option label="区域二" value="beijing"></el-option>
//            </el-select>
//          </el-form-item>
//          <el-form-item label="活动时间">
//            <el-col :span="11">
//              <el-date-picker type="date" laceholder="选择日期v-model="form.date1" style="width: 00%;"></el-date-picker>
//            </el-col>
//            <el-col class="line" :span="2">-</el-col>
//            <el-col :span="11">
//              <el-time-picker placeholder="选择时间" -model="form.date2format="HH:mm:ss" value-format="H:m:s" tyle="width100%;"></el-time-picker>
//            </el-col>
//          </el-form-item>
//          <el-form-item label="即时配送">
//            <el-switch v-model="form.delivery"></el-switch>
//          </el-form-item>
//          <el-form-item label="活动性质">
//            <el-checkbox-group v-model="form.type">
//              <el-checkbox label="美食/餐厅线上活动" name="type"></el-checkbox>
//              <el-checkbox label="地推活动" name="type"></el-checkbox>
//              <el-checkbox label="线下主题活动" name="type"></el-checkbox>
//              <el-checkbox label="单纯品牌曝光" name="type"></el-checkbox>
//            </el-checkbox-group>
//          </el-form-item>
//          <el-form-item label="特殊资源">
//            <el-radio-group v-model="form.resource">
//              <el-radio label="线上品牌商赞助"></el-radio>
//              <el-radio label="线下场地免费"></el-radio>
//            </el-radio-group>
//          </el-form-item>
//          <el-form-item label="活动形式">
//            <el-input type="textarea" v-model="form.desc"></el-input>
//          </el-form-item>
//          <el-form-item>
//            <el-button type="primary" @click="onSubmit">立即创建</el-button>
//            <el-button>取消</el-button>
//          </el-form-item>
//        </el-form>
//    </div>                                                 
//</template>
//export default {
//    props: ['id'],
//    data:function(){
//        return {
//            form:{
//                name:'1',
//                region:'beijing',
//                date1:'2019-02-17',
//                date2:'15:01:01',
//                delivery:true,
//                type:['地推活动','线下主题活动'],
//                resource:'线下场地免费',
//                desc:'活动形式'
//            }
//        }
//    },
//    methods:{
//        onSubmit:function(){
//            console.log(this.form);
//        }
//    }
//}
var Vuecomponent = Vue.component("vform",{
    props: ['id'],
    data:function(){
        return {
            form:{
                name:'1',
                region:'beijing',
                date1:'2019-02-17',
                date2:'15:01:01',
                delivery:true,
                type:['地推活动','线下主题活动'],
                resource:'线下场地免费',
                desc:'活动形式'
            }
        }
    },
    methods:{
        onSubmit:function(){
            console.log(this.form);
        }
    },
    template: '<div :id="id"><el-form ref="form" :model="form" label-width="80px">'+
              '      <el-form-item label="活动名称">'+
              '        <el-input v-model="form.name"></el-input>'+
              '  </el-form-item>'+
              '  <el-form-item label="活动区域">'+
              '    <el-select v-model="form.region" placeholder="请选择活动区域">'+
              '      <el-option label="区域一" value="shanghai"></el-option>'+
              '      <el-option label="区域二" value="beijing"></el-option>'+
              '    </el-select>'+
              '  </el-form-item>'+
              '  <el-form-item label="活动时间">'+
              '    <el-col :span="11">'+
              '      <el-date-picker type="date" placeholder="选择日期" v-model="form.date1" style="width: 100%;"></el-date-picker>'+
              '    </el-col>'+
              '    <el-col class="line" :span="2">-</el-col>'+
              '    <el-col :span="11">'+
              '      <el-time-picker placeholder="选择时间" v-model="form.date2" format="HH:mm:ss" value-format="H:m:s" style="width: 100%;"></el-time-picker>'+
              '    </el-col>'+
              '  </el-form-item>'+
              '  <el-form-item label="即时配送">'+
              '    <el-switch v-model="form.delivery"></el-switch>'+
              '  </el-form-item>'+
              '  <el-form-item label="活动性质">'+
              '    <el-checkbox-group v-model="form.type">'+
              '      <el-checkbox label="美食/餐厅线上活动" name="type"></el-checkbox>'+
              '      <el-checkbox label="地推活动" name="type"></el-checkbox>'+
              '      <el-checkbox label="线下主题活动" name="type"></el-checkbox>'+
              '      <el-checkbox label="单纯品牌曝光" name="type"></el-checkbox>'+
              '    </el-checkbox-group>'+
              '  </el-form-item>'+
              '  <el-form-item label="特殊资源">'+
              '    <el-radio-group v-model="form.resource">'+
              '      <el-radio label="线上品牌商赞助"></el-radio>'+
              '      <el-radio label="线下场地免费"></el-radio>'+
              '    </el-radio-group>'+
              '  </el-form-item>'+
              '  <el-form-item label="活动形式">'+
              '    <el-input type="textarea" v-model="form.desc"></el-input>'+
              '  </el-form-item>'+
              '  <el-form-item>'+
              '    <el-button type="primary" @click="onSubmit">立即创建</el-button>'+
              '    <el-button>取消</el-button>'+
              '  </el-form-item>'+
              '</el-form></div>'
});