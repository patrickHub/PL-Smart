package ch.pristane.laverie.smart.domain.enums;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonValue;

public enum MachineStatus {
    AVAILABLE("Available"),
    RUNNING("Running"),
    OUT_OF_ORDER("OutOfOrder");

    private final String value;

    MachineStatus(String value) {
        this.value = value;
    }

    @JsonValue
    public String getValue() {
        return value;
    }

    @JsonCreator
    public static MachineStatus fromValue(String value) {
        for (MachineStatus status : values()) {
            if (status.value.equalsIgnoreCase(value)) {
                return status;
            }
        }
        throw new IllegalArgumentException("Invalid MachineStatus: " + value);
    }
}
