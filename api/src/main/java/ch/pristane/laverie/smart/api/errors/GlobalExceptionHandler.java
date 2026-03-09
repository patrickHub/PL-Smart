package ch.pristane.laverie.smart.api.errors;

import ch.pristane.laverie.smart.api.contracts.ApiErrorResponse;
import ch.pristane.laverie.smart.application.exceptions.BusinessRuleException;
import ch.pristane.laverie.smart.application.exceptions.NotFoundException;
import ch.pristane.laverie.smart.application.exceptions.ValidationException;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.http.converter.HttpMessageNotReadableException;

import java.util.LinkedHashMap;
import java.util.Map;

@RestControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<ApiErrorResponse> handleMethodArgumentNotValid(
            MethodArgumentNotValidException ex,
            HttpServletRequest request
    ) {
        Map<String, String[]> errors = new LinkedHashMap<>();

        ex.getBindingResult().getFieldErrors().forEach(error ->
                errors.put(error.getField(), new String[]{error.getDefaultMessage()})
        );

        return ResponseEntity.badRequest().body(
                new ApiErrorResponse(
                        "Validation failed",
                        400,
                        "One or more validation errors occurred.",
                        errors,
                        request.getRequestURI()
                )
        );
    }

    @ExceptionHandler(ValidationException.class)
    public ResponseEntity<ApiErrorResponse> handleValidationException(
            ValidationException ex,
            HttpServletRequest request
    ) {
        return ResponseEntity.badRequest().body(
                new ApiErrorResponse(
                        "Validation failed",
                        400,
                        ex.getMessage(),
                        ex.getErrors(),
                        request.getRequestURI()
                )
        );
    }

    @ExceptionHandler(NotFoundException.class)
    public ResponseEntity<ApiErrorResponse> handleNotFoundException(
            NotFoundException ex,
            HttpServletRequest request
    ) {
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(
                new ApiErrorResponse(
                        "Resource not found",
                        404,
                        ex.getMessage(),
                        null,
                        request.getRequestURI()
                )
        );
    }

    @ExceptionHandler(BusinessRuleException.class)
    public ResponseEntity<ApiErrorResponse> handleBusinessRuleException(
            BusinessRuleException ex,
            HttpServletRequest request
    ) {
        return ResponseEntity.status(HttpStatus.CONFLICT).body(
                new ApiErrorResponse(
                        "Business rule violation",
                        409,
                        ex.getMessage(),
                        null,
                        request.getRequestURI()
                )
        );
    }

    @ExceptionHandler(HttpMessageNotReadableException.class)
    public ResponseEntity<ApiErrorResponse> handleHttpMessageNotReadable(
                HttpMessageNotReadableException ex,
                HttpServletRequest request
        ) {
        return ResponseEntity.badRequest().body(
                new ApiErrorResponse(
                        "Invalid request payload",
                        400,
                        "One or more fields have an invalid format.",
                        null,
                        request.getRequestURI()
                )
        );
        }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ApiErrorResponse> handleException(
            Exception ex,
            HttpServletRequest request
    ) {
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(
                new ApiErrorResponse(
                        "Internal Server Error",
                        500,
                        "An unexpected error occurred.",
                        null,
                        request.getRequestURI()
                )
        );
    }
}