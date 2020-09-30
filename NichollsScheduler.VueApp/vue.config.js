module.exports = {
  configureWebpack: {
    devtool: 'source-map'
  },
  devServer: {
    proxy: 'https://localhost:5001'
  }, 
  "transpileDependencies": [
    "vuetify"
  ]
}