import Vue from 'vue';
import Vuetify from 'vuetify/lib';

Vue.use(Vuetify);

const vuetify = new Vuetify({
    theme: {
        themes: {
            light: {
                primary: '#A6192E',
                secondary: '#7C878E',
                white: '#FFFFFF',
                accent: '#2D2926',
                error: '#b71c1c',  
            },
            dark: {
                secondary: '#7C878E'
            }
        }
    }
})

export default vuetify;
