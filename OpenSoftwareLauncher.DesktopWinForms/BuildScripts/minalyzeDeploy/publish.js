require('dotenv').config({
    path: './prod.env'
})
const util = require('util')
const fs = require('fs')
const https = require('https')
const path = require('path')
const axios = require('axios')
const tar = require('tar')
const AWS = require('aws-sdk')
const archiver = require('archiver')

const s3 = new AWS.S3({
    Bucket: process.env['AWS_BUCKET'],
    CreateBucketConfiguration: {
        LocationConstraint: process.env['AWS-LOCATION']
    },
    accessKeyId: process.env['AWS_ID'],
    secretAccessKey: process.env['AWS_SECRET']
})

let artifactsPath = path.resolve('../release/')
async function createTarball () {
    let tarballLocation = path.resolve('./artifacts/osldesktop-win-amd64.zip')
    if (!fs.existsSync(path.dirname(tarballLocation)))
        fs.mkdirSync(path.dirname(tarballLocation), {recursive: true})
    if (fs.existsSync(tarballLocation))
        fs.unlinkSync(tarballLocation)
    await tar.create({
        file: tarballLocation,
        cwd: path.resolve(artifactsPath),
        gzip: false
    }, fs.readdirSync(artifactsPath).filter(v => !v.startsWith('.') || !v.endsWith('pdb')))
    return tarballLocation
}
function zipDirectory(fullPath, outputFilename) {
    if (!fs.existsSync(path.dirname(outputFilename)))
        fs.mkdirSync(path.dirname(outputFilename))
    if (fs.existsSync(outputFilename))
        fs.unlinkSync(outputFilename)
    let outputFilestream = fs.createWriteStream(outputFilename)
    let archive = archiver('zip')
    return new Promise((resolve) => {
        outputFilestream.on('close', () => {
            resolve(outputFilename)
        })
        archive.pipe(outputFilestream)
        archive.directory(fullPath, false)
        archive.finalize()
    })
}
async function s3upload (uploadParameters) {
    let changelogContent = fs.readFileSync('../../../CHANGELOG.txt').toString()
    changelogContent = `<pre><code>${changelogContent}</code></pre>`
    fs.writeFileSync('./artifacts/CHANGELOG.html', changelogContent)
    let fileLocations = fs.readdirSync(path.resolve('./artifacts/')).filter(f => !path.basename(f).startsWith('.'))
    console.log(fileLocations)
    let base = `${uploadParameters.organization}/${uploadParameters.product}/${uploadParameters.timestamp}/`
    let items = []
    for (let i = 0; i < fileLocations.length; i++) {
        let location = path.resolve(`./artifacts/${fileLocations[i]}/`)
        console.log(`uploading ${location}`)
        let stats = await fs.statSync(location)

        let target = `${base}${fileLocations[i]}`

        let content = fs.createReadStream(location)

        let params = {
            Bucket: process.env['AWS_BUCKET'],
            Key: target,
            Body: content,
            path: base,
            ACL: 'public-read'
        }
        if (stats.size > 131072)
            params.StorageClass = 'INTELLIGENT_TIERING'

        let thing = s3.upload(params)
        thing.on('httpUploadProgress', (prog) => {
            let progressPercentage = Math.round(prog.loaded / prog.total * 100);
            console.log(`${progressPercentage}%`);
        })

        let p = await thing.promise()
        
        console.log(p)
        items.push(p)
    }
    return items
}
let commitHash = process.env['CI_COMMIT_SHA'] || require('child_process').execSync(`git rev-parse HEAD`).toString().split('\n')[0]
let commitHashShort = process.env['CI_COMMIT_SHORT_SHA'] || require('child_process').execSync(`git rev-parse --short HEAD`).toString().split('\n')[0]
function fetchTargetVersion()
{
    let versionInfoPath = path.resolve('../../Properties/AssemblyInfo.cs')
    let versionInfoContent = fs.readFileSync(versionInfoPath).toString()
    console.log(versionInfoContent)
    let regex = versionInfoContent.match(/AssemblyFileVersion\("(([0-9]{1,}\.{0,1}){1,})"\)/)
    if (regex != null)
    {
        if (regex[1] != undefined)
        {
            return regex[1]
        }
    }
    console.log(regex)
    process.exit(1)
    return ""
}
async function logic()
{
    let token = process.env['CI_UPLOADTOKEN'] || ''
	console.log(artifactsPath)
    if (!fs.existsSync('./artifacts/'))
        fs.mkdirSync('./artifacts/', { recursive: true })
    await zipDirectory(artifactsPath, path.resolve(`./artifacts/osldesktop-win-amd64.zip`))
    
    fs.writeFileSync('../release/config.ini',
    [
`[Authentication]`,
`Username = `,
`Token = `,
`Remember = True`,
`[Connection]`,
`Endpoint = http://buildservice.api.minalogger.com`,
`[General]`,
`Language = en`,
`ShowLatestRelease = False`
    ].join('\r\n'))

    let timestamp = parseInt(process.env['CI_COMMIT_TIMESTAMP'] == undefined ? Date.now().toString() : (new Date(process.env['CI_COMMIT_TIMESTAMP'] ?? '')).getTime())
    let uploadParameters = {
        organization: 'osl',
        product: 'osldesktop',
        branch: process.env['CI_COMMIT_BRANCH'] || 'main',
        timestamp,
        releaseInfo: {
            version: fetchTargetVersion(),
            name: 'osldesktop',
            envtimestamp: timestamp,
            timestamp,
            productName: 'OSL Desktop',
            appID: 'pet.kate.osl.desktop',
            files: {
                windows: 'osldesktop-win-amd64.zip',
                linux: ''
            },
            executable: {
                windows: 'OpenSoftwareLauncher.DesktopWinForms.exe',
                linux: ''
            },
            commitHash,
            commitHashShort,
            remoteLocation: 'osl/osldesktop'
        },
        files: []
    }
    uploadParameters.releaseInfo.remoteLocation = `osl/${uploadParameters.product}-${uploadParameters.branch}`
    if (process.env['CI_COMMIT_BRANCH'] != undefined && process.env['CI_COMMIT_BRANCH'] != 'main') {
        uploadParameters.product = uploadParameters.product + '-' + process.env['CI_COMMIT_BRANCH']
    }
    console.log(uploadParameters)
    let files = await s3upload(uploadParameters)
    uploadParameters.files = files.map((v) => {
        return {
            ...v,
            ETag: v.ETag.replaceAll('"', ''),
            Location: `https://d1k1slu6p5atxv.cloudfront.net/${v.Key}`
        }
    })
    let targetURL = `${process.env['CI_UPLOADENDPOINT']}?token=${encodeURIComponent(token)}`
    console.log(targetURL)
    const httpsAgent = new https.Agent({
        requestCert: true,
        rejectUnauthorized: false
    })
    axios({
        httpsAgent,
        method: 'POST',
        url: targetURL,
        data: JSON.stringify(uploadParameters),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then((d) => {
        console.log('================================================================ SUCCESS OUTPUT START')
        console.log(util.inspect(d, { showHidden: true, depth: null, colorize: true }))
        if (d.toJSON != undefined) {
            console.log('================================================================ JSON OUTPUT START')
            console.log(util.inspect(d.toJSON(), { showHidden: true, depth: null, colorize: true }))
            console.log('================================================================ JSON OUTPUT END')
        }
        console.log('================================================================ SUCCESS OUTPUT END')
    }).catch((e) => {
        console.log('================================================================ FAIL OUTPUT START')
        console.log(util.inspect(e, { showHidden: true, depth: null, colorize: true }))
        if (e.toJSON != undefined) {
            console.log('================================================================ JSON OUTPUT START')
            console.log(util.inspect(e.toJSON(), { showHidden: true, depth: null, colorize: true }))
            console.log('================================================================ JSON OUTPUT END')
        }
        console.log('================================================================ FAIL OUTPUT END')
    })
}
logic()