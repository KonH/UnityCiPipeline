process.on('unhandledRejection', up => { throw up });

const puppeteer = require('puppeteer');
const fs = require('fs');

try {
    fs.mkdirSync('debug_images');
    console.log('created debug folder.');

} catch (e) {
    console.log('debug folder already present.');
}


(async () => {

    const browser = await puppeteer.launch({
        args: ["--no-sandbox"]
    });
    const page = await browser.newPage();

    console.log('open manual page & wait for login redirect');

	await page.goto('https://license.unity3d.com/manual');

	const mailInputSelector = '#conversations_create_session_form_email',
		  passInputSelector = '#conversations_create_session_form_password';

	await page.waitForSelector(mailInputSelector);

	await page.screenshot({ path: 'debug_images/00_loaded_page.png' });

	console.log('enter credentials');

	await page.type(mailInputSelector, process.env.UNITY_USERNAME);
	await page.type(passInputSelector, process.env.UNITY_PASSWORD);

	await page.screenshot({ path: 'debug_images/01_entered_credentials.png' });

    console.log('click submit');

	await page.click('input[name=commit]');

	await page.screenshot({ path: 'debug_images/02_pressed_commit.png' });

    console.log('wait for license upload form');

	const licenseUploadfield = '#licenseFile';

	await page.waitForSelector(licenseUploadfield);

	await page.screenshot({ path: 'debug_images/03_opened_form.png' });

    console.log('enable interception');
    
	await page.setRequestInterception(true);

    console.log('upload license');

	page.once("request", interceptedRequest => {
		
        interceptedRequest.continue({
            method: "POST",
            postData: fs.readFileSync("unity3d.alf", 'utf8'),
            headers: { "Content-Type": "text/xml" },
        });

	});

	await page.goto('https://license.unity3d.com/genesis/activation/create-transaction');

	await page.screenshot({ path: 'debug_images/04_created_transaction.png' });

    console.log('set license to be personal');

    page.once("request", interceptedRequest => {
        interceptedRequest.continue({
            method: "PUT",
            postData: JSON.stringify({ transaction: { serial: { type: "personal" } } }),
            headers: { "Content-Type": "application/json" }
        });
    });

	await page.goto('https://license.unity3d.com/genesis/activation/update-transaction');

    await page.screenshot({ path: 'debug_images/04_updated_transaction.png' });
    
    console.log('get license content');

    page.once("request", interceptedRequest => {
        interceptedRequest.continue({
            method: "POST",
            postData: JSON.stringify({}),
            headers: { "Content-Type": "application/json" }
        });
    });

    page.on('response', async response => {  
                
        console.log('write license');

        try {
            const data = await response.text();
            const dataJson = await JSON.parse(data);
            fs.writeFileSync("Unity_lic.ulf", dataJson.xml);
            console.log('license file written.');

            await page.screenshot({ path: 'debug_images/05_received_license.png' });
            
        } catch (e) {
            console.log(e);
            console.log('failed to write license file.');
        }

    });

    await page.goto('https://license.unity3d.com/genesis/activation/download-license');
    await page.waitFor(1000);
    await browser.close();
})();