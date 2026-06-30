const https = require('https');

const priests = [
  'Morokei', 'Vokun', 'Nahkriin', 'Namir', 'Dukaan', 'Golnak',
  'Lokir', 'Konahrik', 'Felkir', 'Borngar', 'Hunlwig', 'Volskyrg',
  'Krosis', 'Vahlok the Jailor'
];

async function fetchPage(title) {
  return new Promise((resolve, reject) => {
    const options = {
      hostname: 'skyrim.fandom.com',
      path: `/w/api.php?action=query&format=json&titles=${encodeURIComponent(title)}&prop=revisions&rvprop=content&rvslots=*`,
      headers: { 'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', 'Accept': 'application/json' }
    };
    let body = '';
    const req = https.get(options, (res) => {
      body = '';
      res.on('data', c => body += c);
      res.on('end', () => resolve(body));
    });
    req.on('error', e => reject(e.message));
  });
}

async function main() {
  for (const name of priests) {
    try {
      const raw = await fetchPage(name);
      const j = JSON.parse(raw);
      const pages = j.query.pages;
      for (const pid of Object.keys(pages)) {
        const page = pages[pid];
        if (page.missing) {
          console.log(`=== ${name} === [PAGE MISSING]`);
        } else if (page.revisions && page.revisions[0]) {
          const text = page.revisions[0]['*'];
          console.log(`=== ${name} ===`);
          console.log(text.substring(0, 2500));
          console.log('---END---\n');
        } else {
          console.log(`=== ${name} === [NO REVISIONS]`);
        }
      }
    } catch (e) {
      console.log(`=== ${name} === ERROR: ${e.message}`);
    }
  }
}

main();
