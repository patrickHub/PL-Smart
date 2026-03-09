package ch.pristane.laverie.smart.api;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.webmvc.test.autoconfigure.AutoConfigureMockMvc;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import java.time.OffsetDateTime;
import java.util.Map;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
class MachineStatusIntegrationTest {

    @Autowired
    MockMvc mockMvc;

    @Autowired
    ObjectMapper objectMapper;

    @Test
    void shouldReturnConflictWhenSettingOutOfOrderDuringActiveBooking() throws Exception {
        String machineResponse = mockMvc.perform(post("/api/machines")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(Map.of(
                                "name", "Washer #1",
                                "pricePerCycle", 6.00
                        ))))
                .andExpect(status().isCreated())
                .andReturn()
                .getResponse()
                .getContentAsString();

        String machineId = objectMapper.readTree(machineResponse).get("id").asText();

        mockMvc.perform(post("/api/bookings")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(Map.of(
                                "machineId", machineId,
                                "startTime", OffsetDateTime.now().toString(),
                                "endTime", OffsetDateTime.now().plusHours(1).toString(),
                                "customerName", "Patrick"
                        ))))
                .andExpect(status().isCreated());

        mockMvc.perform(post("/api/machines/" + machineId + "/status")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(Map.of(
                                "status", "OutOfOrder"
                        ))))
                .andExpect(status().isConflict());
    }
}