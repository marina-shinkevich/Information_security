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
const express_1 = __importDefault(require("express"));
const upload_1 = __importDefault(require("./upload"));
const lsb_1 = require("./lsb");
const path_1 = require("path");
const app = (0, express_1.default)();
const PORT = 3000;
app.set('view engine', 'ejs');
app.use(express_1.default.urlencoded({ extended: true }));
app.get('/', (req, res) => {
    res.redirect('/embedded');
});
app.get('/embedded', (req, res) => {
    res.render('embedded');
});
app.post('/embedded', upload_1.default.single('image'), (req, res) => __awaiter(void 0, void 0, void 0, function* () {
    if (!req.file) {
        return res.status(400).send('No file uploaded.');
    }
    const filePath = req.file.path;
    const fileName = (0, path_1.basename)(req.file.filename);
    const extName = (0, path_1.extname)(req.file.filename);
    const message = req.body.message;
    const lsbOutputPath = (0, path_1.join)('files', fileName + 'embedded' + extName);
    yield (0, lsb_1.embedMessage)(filePath, message, lsbOutputPath, req.body.method);
    let result = yield (0, lsb_1.extractMessage)(lsbOutputPath, req.body.method);
    if (req.body.matrix !== undefined) {
        const matrixOriginalOutputPath = (0, path_1.join)('files', fileName + 'origmatrix' + extName);
        const matrixOutputPath = (0, path_1.join)('files', fileName + 'embmatrix' + extName);
        (0, lsb_1.getColorMatrix)(filePath, matrixOriginalOutputPath);
        (0, lsb_1.getColorMatrix)(lsbOutputPath, matrixOutputPath);
    }
    return res.status(200).json({ result });
}));
app.listen(PORT, () => {
    console.log(`Server is running at http://localhost:${PORT}`);
});
