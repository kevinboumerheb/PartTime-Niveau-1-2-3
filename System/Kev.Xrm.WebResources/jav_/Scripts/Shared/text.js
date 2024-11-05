var Javista = Javista || {};
Javista.Utils = Javista.Utils || {};
Javista.Utils.Text = function () {
    function toUpperCase(formContext, attributeName) {
        var attr = formContext.getAttribute(attributeName);
        if (!attr) return;

        attr.setValue(attr.getValue().toUpperCase);
    }

    function alphabetLatinCheck(formContext, attributeName, attributeNameTwo, compositeField) {
        var regexObj = /^([a-zA-Z0-9_\.\-\ \�\�\�\�\�\'\,\�\�\�\"])+$/;

        if (attributeNameTwo != undefined && compositeField != undefined) {
            var fieldvalue = formContext.getAttribute(attributeName).getValue();
            var fieldvalue2 = formContext.getAttribute(attributeNameTwo).getValue();

            //V�rification que le champs ne contient que des caract�res inclus dans l'expression r�guli�re
            //Si oui: la notification est enlev�e
            //Si non: une notification est rajout�e, ce qui bloque le champ et l'enregistrement
            if (regexObj.test(fieldvalue) & regexObj.test(fieldvalue2)) {
                formContext.getControl(compositeField).clearNotification();
            }
            else {
                formContext.getControl(compositeField).setNotification("Le champ doit contenir uniquement des caract�res latin");
            }
        }
        else {
            var fieldvalue = formContext.getAttribute(attributeName).getValue();

            //V�rification que le champs ne contient que des caract�res inclus dans l'expression r�guli�re
            //Si oui: la notification est enlev�e
            //Si non: une notification est rajout�e, ce qui bloque le champ et l'enregistrement
            if (regexObj.test(fieldvalue)) {
                formContext.getControl(attributeName).clearNotification();
            }
            else {
                formContext.getControl(attributeName).setNotification("Le champ doit contenir uniquement des caract�res latin");
            }
        }
    }

    return {
        ToUpperCase: toUpperCase,
        AlphabetLatinCheck: alphabetLatinCheck
    };
}();