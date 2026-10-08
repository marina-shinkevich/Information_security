"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.convertBinaryToString = exports.extractMessage = exports.embedMessage = exports.getColorMatrix = exports.Method = void 0;
// @ts-ignore
const jimp_1 = __importDefault(require("jimp"));
var Method;
(function (Method) {
    Method["ROWS"] = "rows";
    Method["COLS"] = "cols";
})(Method || (exports.Method = Method = {}));
const getColorMatrix = (imagePath, outputPath) => __awaiter(void 0, void 0, void 0, function* () {
    const image = yield jimp_1.default.read(imagePath);
    const newImage = new jimp_1.default(image.getWidth(), image.getHeight());
    image.scan(0, 0, image.getWidth(), image.getHeight(), (x, y, idx) => {
        const red = image.bitmap.data[idx];
        const green = image.bitmap.data[idx + 1];
        const blue = image.bitmap.data[idx + 2];
        const newRed = red & 0x01;
        const newGreen = green & 0x01;
        const newBlue = blue & 0x01;
        newImage.setPixelColor(jimp_1.default.rgbaToInt(newRed * 255, newGreen * 255, newBlue * 255, 255), x, y);
    });
    yield newImage.writeAsync(outputPath);
});
exports.getColorMatrix = getColorMatrix;
const embedMessage = (containerPath, message, outputImagePath, method) => __awaiter(void 0, void 0, void 0, function* () {
    const containerImage = yield jimp_1.default.read(containerPath);
    const messageBytes = Buffer.from(message, 'utf8');
    const messageBits = buf2bin(messageBytes);
    const maxMessageBits = (containerImage.getWidth() * containerImage.getHeight()) * 3;
    if (messageBits.length > maxMessageBits)
        throw new Error("Message is too large for the container");
    let messageBitIndex = 0;
    let endZeroGroup = 0;
    let xMax = 0;
    let yMax = 0;
    switch (method) {
        case Method.ROWS:
            xMax = containerImage.getWidth();
            yMax = containerImage.getHeight();
            break;
        case Method.COLS:
            xMax = containerImage.getHeight();
            yMax = containerImage.getWidth();
            break;
    }
    for (let y = 0; y < yMax; y++) {
        for (let x = 0; x < xMax; x++) {
            let pixelColor = 0;
            switch (method) {
                case Method.ROWS:
                    pixelColor = containerImage.getPixelColor(x, y);
                    break;
                case Method.COLS:
                    pixelColor = containerImage.getPixelColor(y, x);
                    break;
            }
            const red = jimp_1.default.intToRGBA(pixelColor).r;
            const green = jimp_1.default.intToRGBA(pixelColor).g;
            const blue = jimp_1.default.intToRGBA(pixelColor).b;
            let newRed = 0;
            let newGreen = 0;
            let newBlue = 0;
            if (messageBitIndex < messageBits.length) {
                if (+messageBits[messageBitIndex]) {
                    newRed = red | 1;
                }
                else {
                    newRed = red & ~1;
                }
                messageBitIndex++;
                if (messageBitIndex < messageBits.length) {
                    if (+messageBits[messageBitIndex]) {
                        newGreen = green | 1;
                    }
                    else {
                        newGreen = green & ~1;
                    }
                    messageBitIndex++;
                }
                if (messageBitIndex < messageBits.length) {
                    if (+messageBits[messageBitIndex]) {
                        newBlue = blue | 1;
                    }
                    else {
                        newBlue = blue & ~1;
                    }
                    messageBitIndex++;
                }
                switch (method) {
                    case Method.ROWS:
                        containerImage.setPixelColor(jimp_1.default.rgbaToInt(newRed, newGreen, newBlue, 255), x, y);
                        break;
                    case Method.COLS:
                        containerImage.setPixelColor(jimp_1.default.rgbaToInt(newRed, newGreen, newBlue, 255), y, x);
                        break;
                }
            }
            else {
                if (endZeroGroup >= 3) {
                    break;
                }
                else {
                    newRed = red & ~1;
                    newGreen = green & ~1;
                    newBlue = blue & ~1;
                    switch (method) {
                        case Method.ROWS:
                            containerImage.setPixelColor(jimp_1.default.rgbaToInt(newRed, newGreen, newBlue, 255), x, y);
                            break;
                        case Method.COLS:
                            containerImage.setPixelColor(jimp_1.default.rgbaToInt(newRed, newGreen, newBlue, 255), y, x);
                            break;
                    }
                    endZeroGroup++;
                }
            }
        }
        if (messageBitIndex >= messageBits.length) {
            break;
        }
    }
    yield containerImage.writeAsync(outputImagePath);
});
exports.embedMessage = embedMessage;
const extractMessage = (imagePath, method) => __awaiter(void 0, void 0, void 0, function* () {
    const containerImage = yield jimp_1.default.read(imagePath);
    let messageBits = "";
    const messageBitsLength = (containerImage.getWidth() * containerImage.getHeight()) * 3;
    let messageBitIndex = 0;
    let endZeroGroup = 0;
    let xMax = 0;
    let yMax = 0;
    switch (method) {
        case Method.ROWS:
            xMax = containerImage.getWidth();
            yMax = containerImage.getHeight();
            break;
        case Method.COLS:
            xMax = containerImage.getHeight();
            yMax = containerImage.getWidth();
            break;
    }
    for (let y = 0; y < yMax; y++) {
        for (let x = 0; x < xMax; x++) {
            let pixelColor = 0;
            switch (method) {
                case Method.ROWS:
                    pixelColor = containerImage.getPixelColor(x, y);
                    break;
                case Method.COLS:
                    pixelColor = containerImage.getPixelColor(y, x);
                    break;
            }
            const red = jimp_1.default.intToRGBA(pixelColor).r;
            const green = jimp_1.default.intToRGBA(pixelColor).g;
            const blue = jimp_1.default.intToRGBA(pixelColor).b;
            messageBits += red & 1;
            messageBitIndex++;
            if (messageBitIndex < messageBitsLength) {
                messageBits += green & 1;
                messageBitIndex++;
            }
            if (messageBitIndex < messageBitsLength) {
                messageBits += blue & 1;
                messageBitIndex++;
            }
            if (messageBits.slice(messageBits.length - 3) === "000") {
                endZeroGroup++;
            }
            else {
                endZeroGroup = 0;
            }
            if (messageBitIndex >= messageBitsLength) {
                break;
            }
            if (endZeroGroup >= 3) {
                break;
            }
        }
        if (messageBitIndex >= messageBitsLength) {
            break;
        }
        if (endZeroGroup >= 3) {
            break;
        }
    }
    return (0, exports.convertBinaryToString)(messageBits);
});
exports.extractMessage = extractMessage;
const buf2bin = (buffer) => {
    return BigInt('0x' + buffer.toString('hex')).toString(2).padStart(buffer.length * 8, '0');
};
const convertBinaryToString = (binaryString) => {
    let text = '';
    for (let i = 0; i < binaryString.length; i += 8) {
        const byte = binaryString.slice(i, i + 8);
        const charCode = parseInt(byte, 2);
        const char = String.fromCharCode(charCode);
        text += char;
    }
    return text;
};
exports.convertBinaryToString = convertBinaryToString;
