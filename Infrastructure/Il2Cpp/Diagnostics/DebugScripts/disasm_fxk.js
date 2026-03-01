// Disassemble MapCreator.fxk to understand tile ID decoding
const mod = Process.getModuleByName("GameAssembly.dll");
const fxkAddr = ptr("0x254e1e170f0");

// Read 256 bytes of native code at fxk
const bytes = fxkAddr.readByteArray(256);
const u8 = new Uint8Array(bytes);
const lines = [];
for (let i = 0; i < u8.length; i += 16) {
    const hex = [];
    for (let j = i; j < Math.min(i+16, u8.length); j++) {
        hex.push(('0'+u8[j].toString(16)).slice(-2));
    }
    lines.push((fxkAddr.add(i)) + ': ' + hex.join(' '));
}
console.log(lines.join('\n'));

// Also try to use Instruction.parse for disassembly
console.log("\n=== Disassembly ===");
let addr = fxkAddr;
for (let i = 0; i < 60; i++) {
    try {
        const insn = Instruction.parse(addr);
        console.log(addr + ': ' + insn.toString());
        addr = insn.next;
    } catch(e) {
        console.log(addr + ': (parse error)');
        break;
    }
}
