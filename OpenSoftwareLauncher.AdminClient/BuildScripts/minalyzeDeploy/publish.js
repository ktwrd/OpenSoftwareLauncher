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
let localCommitHash = require('child_process').execSync(`git rev-parse HEAD`).toString().split('\n')[0].toString()
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
function safeGet(value, fallback)
{
    if (value == undefined || value == null || (typeof value == 'string' && value.length < 1))
        return fallback
    return value
}
const parameters = {
    timestamp: safeGet(process.env['CI_COMMIT_TIMESTAMP'], Date.now().toString()),
    name: 'osladminclient',
    productName: 'OSL Admin Client',
    organization: 'osl',
    appId: 'pet.kate.osl.desktop',
    commitHash: safeGet(process.env['CI_COMMIT_SHA'], localCommitHash),
    commitHashShort: safeGet(process.env['CI_COMMIT_SHA_SHORT'], localCommitHash.substring(0, 7)),
    branch: safeGet(process.env['CI_COMMIT_BRANCH'], 'main'),
    exe: 'OpenSoftwareLauncher.AdminClient.exe'
}
async function logic()
{
    let token = process.env['CI_UPLOADTOKEN'] || ''
	console.log(artifactsPath)
    if (!fs.existsSync('./artifacts/'))
        fs.mkdirSync('./artifacts/', { recursive: true })
    await zipDirectory(artifactsPath, path.resolve(`./artifacts/adminclient-win-amd64.zip`))

    let uploadParameters = {
        organization: parameters.organization,
        product: `${parameters.name}-${parameters.branch}`,
        branch: parameters.branch,
        timestamp: parameters.timestamp,
        releaseInfo: {
            version: fetchTargetVersion(),
            name: parameters.name,
            envtimestamp: parameters.timestamp,
            timestamp: parameters.timestamp,
            productName: parameters.productName,
            appID: parameters.appId,
            executable: {
                windows: parameters.exe,
                linux: ''
            },
            commitHash: parameters.commitHash,
            commitHashShort: parameters.commitHashShort,
            remoteLocation: `${parameters.organization}/${parameters.name}-${parameters.branch}`
        },
        files: []
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
    // Send product to Server
    axios({
        httpsAgent,
        method: 'POST',
        url: targetURL,
        data: JSON.stringify(uploadParameters),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then((d) => {
        console.log(d.status)
        console.log('================================================================ SUCCESS OUTPUT START')
        console.log(util.inspect(d.data, { showHidden: true, depth: null, colorize: true }))
        if (d.toJSON != undefined) {
            console.log(util.inspect(d.toJSON(), { showHidden: true, depth: null, colorize: true }))
        }
        console.log('================================================================ SUCCESS OUTPUT END')
    }).catch((e) => {
        console.log(e.status, e.code)
        console.log('================================================================ FAIL OUTPUT START')
        console.log(util.inspect(e.data, { showHidden: true, depth: null, colorize: true }))
        if (e.toJSON != undefined) {
            console.log(util.inspect(e.toJSON(), { showHidden: true, depth: null, colorize: true }))
        }
        console.log('================================================================ FAIL OUTPUT END')
    })
}
logic()