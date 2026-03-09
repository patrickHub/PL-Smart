package ch.pristane.laverie.smart.application.exceptions;

import java.util.Map;

public class ValidationException extends RuntimeException {
    private final Map<String, String[]> errors;

    public ValidationException(Map<String, String[]> errors) {
        super("One or more validation errors occurred.");
        this.errors = errors;
    }

    public Map<String, String[]> getErrors() {
        return errors;
    }
}