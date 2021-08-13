namespace Toolbelt.Head {
    const searchParam = document.currentScript?.getAttribute('src')?.split('?')[1] || '';
    export var ready = import('./script.module.min.js?' + searchParam).then(m => {
        Object.assign(Head, m.Toolbelt.Head);
    });
}
