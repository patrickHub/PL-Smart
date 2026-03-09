package ch.pristane.laverie.smart.api;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.webmvc.test.autoconfigure.AutoConfigureMockMvc;
import org.springframework.http.MediaType;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.test.web.servlet.MockMvc;

import java.util.Map;

import static org.assertj.core.api.Assertions.assertThat;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
@ActiveProfiles("test")
class MachineAuditIntegrationTest {

    @Autowired
    MockMvc mockMvc;

    @Autowired
    ObjectMapper objectMapper;

    @Test
    void shouldCreateAuditEntryWhenMachineStatusChanges() throws Exception {
        String machineResponse = mockMvc.perform(post("/api/machines")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(Map.of(
                                "name", "Washer Audit",
                                "pricePerCycle", 6.00
                        ))))
                .andExpect(status().isCreated())
                .andReturn()
                .getResponse()
                .getContentAsString();

        String machineId = objectMapper.readTree(machineResponse).get("id").asText();

        mockMvc.perform(post("/api/machines/" + machineId + "/status")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(Map.of(
                                "status", "OutOfOrder"
                        ))))
                .andExpect(status().isNoContent());

        String auditsResponse = mockMvc.perform(get("/api/machines/" + machineId + "/audits"))
                .andExpect(status().isOk())
                .andReturn()
                .getResponse()
                .getContentAsString();

        JsonNode audits = objectMapper.readTree(auditsResponse);

        assertThat(audits.isArray()).isTrue();
        assertThat(audits.size()).isGreaterThan(0);
        assertThat(audits.get(0).get("oldStatus").asText()).isEqualTo("Available");
        assertThat(audits.get(0).get("newStatus").asText()).isEqualTo("OutOfOrder");
    }
}