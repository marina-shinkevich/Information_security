const crypto = require('crypto'); //node --openssl-legacy-provider weak.js

// Слабые ключи
 const weakKeys = [
     Buffer.from([0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01]),
     Buffer.from([0x1F, 0x1F, 0x1F, 0x1F, 0x0E, 0x0E, 0x0E, 0x0E]),
     Buffer.from([0xE0, 0xE0, 0xE0, 0xE0, 0xF1, 0xF1, 0xF1, 0xF1]),
 ];

// Полуслабые ключи
//const semiWeakKeys = [
   // Buffer.from([0x01, 0xFE, 0x01, 0xFE, 0x01, 0xFE, 0x01, 0xFE]),
    //Buffer.from([0x1F, 0xE0, 0x0E, 0xF1, 0xE0, 0x1F, 0xF1, 0x0E]),
    //Buffer.from([0x01, 0xE0, 0x01, 0xF1, 0xE0, 0x01, 0xF1, 0x01]),];

// Функция шифрования
function encrypt(plainText, key) {
    const cipher = crypto.createCipheriv('des-ecb', key, null); // Используем режим ECB
    let encrypted = cipher.update(plainText, 'utf8', 'hex');
    encrypted += cipher.final('hex');
    return encrypted;
}

// Функция расшифрования
function decrypt(cipherText, key) {
    const decipher = crypto.createDecipheriv('des-ecb', key, null);
    let decrypted = decipher.update(cipherText, 'hex', 'utf8');
    decrypted += decipher.final('utf8');
    return decrypted;
}


function getAvalancheEffect(original, modified) {
    let changes = 0;

    for (let i = 0; i < Math.max(original.length, modified.length); i++) {
        const originalChar = original.charCodeAt(i) || 0;
        const modifiedChar = modified.charCodeAt(i) || 0;
        
        const xorResult = originalChar ^ modifiedChar;
    
        changes += xorResult.toString(2).split('0').join('').length;
    }

    const totalBits = original.length * 8; 
    const percentage = (changes+4 / totalBits) * 100; 

    return percentage;
}

const plainText = 'Hello, world!';
console.log('Исходный текст:', plainText);


weakKeys.forEach((key, index) => {
    const encryptedText = encrypt(plainText, key);
    console.log(`Зашифрованный текст (SemiWeakKey${index + 1}):`, encryptedText);

    const decryptedText = decrypt(encryptedText, key);
    console.log(`Расшифрованный текст (SemiWeakKey${index + 1}):`, decryptedText);

    const avalancheEffect = getAvalancheEffect(plainText, decryptedText);
    console.log(`Лавинный эффект при использовании SemiWeakKey${index + 1}: ${avalancheEffect.toFixed(2)}%`);
});