/*
 * Webpack config file
 */
const path = require('path');
const webpack = require('webpack');
const TerserJSPlugin = require('terser-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const CopyPlugin = require('copy-webpack-plugin');

const inProduction = process.env.NODE_ENV === 'production';

const outputDir = path.resolve(__dirname, './wwwroot/assets/');

module.exports = {
    optimization: {
        minimizer: [new TerserJSPlugin({}), new OptimizeCSSAssetsPlugin({})],
    },
    plugins: [
        // extract css files from assets to wwwroot 
        new MiniCssExtractPlugin({
            filename: inProduction ? '[name].[hash].css' : '[name].css',
            chunkFilename: inProduction ? '[id].[hash].css': '[id].css',
        }),
        // copy govuk image/font files to wwwroot
        new CopyPlugin([
            { from: './node_modules/govuk-frontend/assets', to: outputDir }
        ])
    ],
    entry: {
        // js
        'js/init': './assets-src/js/init.js',
        'js/govuk': './assets-src/js/govuk.js',
        'js/validation': './assets-src/js/validation.js',
        // scss
        'css/all': './assets-src/scss/all.scss',
        'css/crit': './assets-src/scss/critical.scss'
    },
    output: {
        path: outputDir,
        filename: '[name].min.js'
    },
    module: {
        rules: [
            {
                test: /\.(sa|sc|c)ss$/, // for any css/sass/scss files
                include: [
                    path.resolve(__dirname, "./assets-src/scss/") // in the assets/scss folder
                ],
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader, // extracts CSS into files
                        options: {
                            hmr: !inProduction, // hot module reloading on dev
                        },
                    },
                    'css-loader', // resolves imports
                    'sass-loader' // compliles scss to css
                ],
            },

            /*
             *   js files
             */
            {
                test: /\.js$/, // for all .js files
                include: [
                    path.resolve(__dirname, "./assets-src/js/") // in the assets/js folder
                ],                
                use: {
                    loader: 'babel-loader', // run babel to convert js to cross browser supported code
                    options: {
                        presets: ['@babel/preset-env'] // using this preset's rules
                    }
                }
            }
        ]
    }
};
