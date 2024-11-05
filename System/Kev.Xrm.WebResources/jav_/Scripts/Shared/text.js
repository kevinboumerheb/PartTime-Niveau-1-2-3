var Javista = Javista || {};
Javista.Utils = Javista.Utils || {};
Javista.Utils.Text = function () {
    function toUpperCase(formContext, attributeName) {
        var attr = formContext.getAttribute(attributeName);
        if (!attr) return;

        attr.setValue(attr.getValue().toUpperCase);
    }

    function alphabetLatinCheck(formContext, attributeName, attributeNameTwo, compositeField) {
        var regexObj = /^([a-zA-Z0-9_\.\-\ \é\à\ç\ù\è\'\,\ê\â\ô\"])+$/;

        if (attributeNameTwo != undefined && compositeField != undefined) {
            var fieldvalue = formContext.getAttribute(attributeName).getValue();
            var fieldvalue2 = formContext.getAttribute(attributeNameTwo).getValue();

            //Vérification que le champs ne contient que des caractères inclus dans l'expression régulière
            //Si oui: la notification est enlevée
            //Si non: une notification est rajoutée, ce qui bloque le champ et l'enregistrement
            if (regexObj.test(fieldvalue) & regexObj.test(fieldvalue2)) {
                formContext.getControl(compositeField).clearNotification();
            }
            else {
                formContext.getControl(compositeField).setNotification("Le champ doit contenir uniquement des caractères latin");
            }
        }
        else {
            var fieldvalue = formContext.getAttribute(attributeName).getValue();

            //Vérification que le champs ne contient que des caractères inclus dans l'expression régulière
            //Si oui: la notification est enlevée
            //Si non: une notification est rajoutée, ce qui bloque le champ et l'enregistrement
            if (regexObj.test(fieldvalue)) {
                formContext.getControl(attributeName).clearNotification();
            }
            else {
                formContext.getControl(attributeName).setNotification("Le champ doit contenir uniquement des caractères latin");
            }
        }
    }

    return {
        ToUpperCase: toUpperCase,
        AlphabetLatinCheck: alphabetLatinCheck
    };
}();